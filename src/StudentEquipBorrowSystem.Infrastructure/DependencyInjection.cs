using Applications;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

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
        /// Applies pending EF Core migrations (creates the database on first run).
        /// Lives in Infrastructure so the Desktop project needs no EF Core types.
        /// </summary>
        public static void InitializeDatabase(this IServiceProvider provider)
        {
            var context = provider.GetRequiredService<EquipmentBorrowingDbContext>();
            context.Database.Migrate();
        }
    }
}