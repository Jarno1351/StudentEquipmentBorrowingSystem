using Applications;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        /// <summary>
        /// Registers SQLite persistence. Application services keep depending only on
        /// IStudentRepository / IEquipmentRepository / IBorrowRepository.
        /// Singleton lifetimes are used because the app services and ViewModels are singletons.
        /// </summary>
        public static IServiceCollection AddSqlitePersistence(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<EquipmentBorrowingDbContext>(
                options => options.UseSqlite(connectionString),
                ServiceLifetime.Singleton,
                ServiceLifetime.Singleton);

            services.AddSingleton<IStudentRepository, EfStudentRepository>();
            services.AddSingleton<IEquipmentRepository, EfEquipmentRepository>();
            services.AddSingleton<IBorrowRepository, EfBorrowRepository>();

            return services;
        }

        /// <summary>
        /// Startup initialization, in two non-destructive steps:
        ///   1. Apply pending EF Core migrations (creates the database on first run, does nothing afterwards).
        ///   2. Add any missing demo data. Existing data is never deleted or overwritten.
        /// Lives in Infrastructure so the Desktop project needs no EF Core types.
        /// </summary>
        public static void InitializeDatabase(this IServiceProvider provider)
        {
            // Runs on a thread-pool thread so blocking here can never deadlock on the UI thread.
            Task.Run(async () =>
            {
                var context = provider.GetRequiredService<EquipmentBorrowingDbContext>();
                await context.Database.MigrateAsync();

                await DemoDataSeeder.SeedAsync(
                    provider.GetRequiredService<IStudentRepository>(),
                    provider.GetRequiredService<IEquipmentRepository>(),
                    provider.GetRequiredService<IBorrowRepository>());
            }).GetAwaiter().GetResult();
        }
    }
}