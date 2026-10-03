using System.ComponentModel;
using StudentBorrowEquipSystem.Desktop.ViewModels;

namespace StudentBorrowEquipSystem.Desktop.Navigation;

public interface INavigationService : INotifyPropertyChanged
{
    ViewModelBase? CurrentViewModel { get; }
    void NavigateTo<TViewModel>() where TViewModel : ViewModelBase;
}
