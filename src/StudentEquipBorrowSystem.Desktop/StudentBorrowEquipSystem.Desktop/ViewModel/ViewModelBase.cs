using CommunityToolkit.Mvvm.ComponentModel;

namespace StudentBorrowEquipSystem.Desktop.ViewModels;

/// Base for page view models: shared success/error feedback + navigation hook.
public abstract partial class ViewModelBase : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasStatus))]
    private string? statusMessage;

    [ObservableProperty]
    private bool isError;

    public bool HasStatus => !string.IsNullOrEmpty(StatusMessage);

    protected void ShowSuccess(string message)
    {
        IsError = false;
        StatusMessage = "✓ " + message;
    }

    protected void ShowError(string message)
    {
        IsError = true;
        StatusMessage = message;
    }

    public void ClearStatus() => StatusMessage = null;

    /// Called by the navigation service every time the page is shown (refresh data here).
    public virtual void OnNavigatedTo() { }
}
