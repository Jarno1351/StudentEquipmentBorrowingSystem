using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Applications;
using Infrastructure;
using StudentBorrowEquipSystem.Desktop.ViewModels;

namespace StudentBorrowEquipSystem.Desktop;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Configure DI
        var services = new ServiceCollection();

        // Infrastructure repositories
        services.AddSingleton<IStudentRepository, StudentInMemoryRepository>();
        services.AddSingleton<IEquipmentRepository, EquipmentInMemoryRepository>();
        services.AddSingleton<IBorrowRepository, BorrowInMemoryRepository>();

        // Domain/application services
        services.AddSingleton<IBorrowService, BorrowService>();
        services.AddSingleton<IBorrowAppService, BorrowAppService>();
        services.AddSingleton<ILookupAppService, LookupAppService>();

        // ViewModels
        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<BorrowingViewModel>();
        services.AddSingleton<ActiveBorrowingsViewModel>();
        services.AddSingleton<EquipmentViewModel>();

        Services = services.BuildServiceProvider();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainVm = Services.GetRequiredService<MainWindowViewModel>();
            desktop.MainWindow = new MainWindow
            {
                DataContext = mainVm
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}