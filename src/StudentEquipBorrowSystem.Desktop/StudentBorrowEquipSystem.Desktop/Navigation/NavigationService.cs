using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using StudentBorrowEquipSystem.Desktop.ViewModels;

namespace StudentBorrowEquipSystem.Desktop.Navigation;

public partial class NavigationService : ObservableObject, INavigationService
{
    private readonly IServiceProvider _services;

    [ObservableProperty]
    private ViewModelBase? currentViewModel;

    public NavigationService(IServiceProvider services) => _services = services;

    public void NavigateTo<TViewModel>() where TViewModel : ViewModelBase
    {
        var vm = _services.GetRequiredService<TViewModel>();
        vm.ClearStatus();
        vm.OnNavigatedTo();          // refresh data each time the page is shown
        CurrentViewModel = vm;
    }
}
