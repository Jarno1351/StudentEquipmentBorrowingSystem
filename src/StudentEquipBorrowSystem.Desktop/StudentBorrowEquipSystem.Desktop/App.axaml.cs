using Applications;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using StudentBorrowEquipSystem.Desktop.Navigation;
using StudentBorrowEquipSystem.Desktop.ViewModels;
using System;
using System.IO;

namespace StudentBorrowEquipSystem.Desktop;

public partial class App : Avalonia.Application
{
    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        // ---- Composition root: the ONLY place dependencies are wired ----
        var provider = ConfigureServices();

        // Startup initialization (Infrastructure): applies migrations and adds any missing
        // demo data. Non-destructive - the database is never recreated or overwritten.
        provider.InitializeDatabase();

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

        // Persistence (Infrastructure). To change storage, change only this one line:
        // swap AddSqlitePersistence for another registration that maps the same three
        // repository interfaces (for example the in-memory repositories).
        var dbPath = Path.Combine(AppContext.BaseDirectory, "EquipmentBorrowing.db");
        services.AddSqlitePersistence($"Data Source={dbPath}");

        // Application services
        services.AddSingleton<IBorrowService, BorrowService>();
        services.AddSingleton<IBorrowAppService, BorrowAppService>();
        services.AddSingleton<ILookupAppService, LookupAppService>();
        services.AddSingleton<StudentService>();

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