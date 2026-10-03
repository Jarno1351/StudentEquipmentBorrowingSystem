using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StudentBorrowEquipSystem.Desktop.Navigation;

namespace StudentBorrowEquipSystem.Desktop.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    public INavigationService Navigation { get; }

    public bool IsEquipmentActive => Navigation.CurrentViewModel is EquipmentViewModel;
    public bool IsBorrowActive => Navigation.CurrentViewModel is BorrowingViewModel;
    public bool IsActiveBorrowingsActive => Navigation.CurrentViewModel is ActiveBorrowingsViewModel;

    public MainWindowViewModel(INavigationService navigation)
    {
        Navigation = navigation;
        Navigation.PropertyChanged += (_, _) =>
        {
            OnPropertyChanged(nameof(IsEquipmentActive));
            OnPropertyChanged(nameof(IsBorrowActive));
            OnPropertyChanged(nameof(IsActiveBorrowingsActive));
        };
        ShowEquipment();
    }

    [RelayCommand] private void ShowEquipment() => Navigation.NavigateTo<EquipmentViewModel>();
    [RelayCommand] private void ShowBorrow() => Navigation.NavigateTo<BorrowingViewModel>();
    [RelayCommand] private void ShowActiveBorrowings() => Navigation.NavigateTo<ActiveBorrowingsViewModel>();
}
