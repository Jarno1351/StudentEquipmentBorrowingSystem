using Applications;
using Applications.Dto;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;

namespace StudentBorrowEquipSystem.Desktop.ViewModels;

public partial class ActiveBorrowingsViewModel : ViewModelBase
{
    private readonly IBorrowAppService _borrowAppService;

    public ObservableCollection<BorrowDto> ActiveBorrowings { get; } = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ValidationMessage))]
    [NotifyPropertyChangedFor(nameof(HasValidationMessage))]
    [NotifyCanExecuteChangedFor(nameof(ReturnSelectedCommand))]
    private BorrowDto? selectedBorrow;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ValidationMessage))]
    [NotifyPropertyChangedFor(nameof(HasValidationMessage))]
    [NotifyCanExecuteChangedFor(nameof(ReturnSelectedCommand))]
    private DateTime? returnDate = DateTime.Today;

    public string? ValidationMessage =>
        SelectedBorrow == null ? "Please select an active borrowing." :
        ReturnDate == null ? "Please select a return date." :
        null;

    public bool HasValidationMessage => ValidationMessage != null;

    public ActiveBorrowingsViewModel(IBorrowAppService borrowAppService)
    {
        _borrowAppService = borrowAppService;
        LoadActiveBorrowings();
    }

    public override void OnNavigatedTo() => LoadActiveBorrowings();

    public void LoadActiveBorrowings()
    {
        ActiveBorrowings.Clear();
        foreach (var b in _borrowAppService.GetActiveBorrowings())
            ActiveBorrowings.Add(b);
    }

    private bool CanReturn() => ValidationMessage == null;

    [RelayCommand(CanExecute = nameof(CanReturn))]
    private void ReturnSelected()
    {
        var problem = ValidationMessage;
        if (problem != null) { ShowError(problem); return; }

        try
        {
            var result = _borrowAppService.ReturnEquipment(SelectedBorrow!.BorrowId, ReturnDate!.Value);

            if (result.Success)
            {
                LoadActiveBorrowings();
                ShowSuccess("Equipment returned successfully.");
            }
            else
            {
                ShowError(result.Message ?? "Return failed.");
            }
        }
        catch (Exception ex)
        {
            ShowError($"Unexpected error: {ex.Message}");
        }
    }
}
