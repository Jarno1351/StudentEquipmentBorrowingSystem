using Applications;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using StudentBorrowEquipSystem.Desktop.Navigation;
using StudentBorrowEquipSystem.Desktop.ViewModels;
using System;

namespace StudentBorrowEquipSystem.Desktop;

public partial class App : Avalonia.Application
{
    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        // ---- Composition root: the ONLY place dependencies are wired ----
        var provider = ConfigureServices();

        // Demo data (seeded through repository interfaces; Domain stays out of Desktop)
        DemoDataSeeder.Seed(
            provider.GetRequiredService<IStudentRepository>(),
            provider.GetRequiredService<IEquipmentRepository>());

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = provider.GetRequiredService<MainWindowViewModel>()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static IServiceProvider ConfigureServices()
    {
        IServiceCollection services = new ServiceCollection();

        // Repositories (Infrastructure)
        services.AddSingleton<IStudentRepository, StudentInMemoryRepository>();
        services.AddSingleton<IEquipmentRepository, EquipmentInMemoryRepository>();
        services.AddSingleton<IBorrowRepository, BorrowInMemoryRepository>();

        // Application services
        services.AddSingleton<IBorrowService, BorrowService>();
        services.AddSingleton<IBorrowAppService, BorrowAppService>();
        services.AddSingleton<ILookupAppService, LookupAppService>();

        // Navigation
        services.AddSingleton<INavigationService, NavigationService>();

        // ViewModels (singletons keep form state when switching pages; data is refreshed in OnNavigatedTo)
        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<EquipmentViewModel>();
        services.AddSingleton<BorrowingViewModel>();
        services.AddSingleton<ActiveBorrowingsViewModel>();

        return services.BuildServiceProvider();
    }
}
