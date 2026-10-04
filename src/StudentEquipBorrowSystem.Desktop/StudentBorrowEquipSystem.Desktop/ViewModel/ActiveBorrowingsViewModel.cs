using Applications;
using Applications.Dto;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

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
        LoadActiveBorrowingsAsync().Wait();
    }

    public override void OnNavigatedTo() => LoadActiveBorrowingsAsync().Wait();

    private async Task LoadActiveBorrowingsAsync()
    {
        ActiveBorrowings.Clear();
        var borrowings = await _borrowAppService.GetActiveBorrowingsAsync();
        foreach (var b in borrowings)
            ActiveBorrowings.Add(b);
    }

    private bool CanReturn() => ValidationMessage == null;

    [RelayCommand(CanExecute = nameof(CanReturn))]
    private async Task ReturnSelected()
    {
        var problem = ValidationMessage;
        if (problem != null) { ShowError(problem); return; }

        try
        {
            var result = await _borrowAppService.ReturnEquipmentAsync(SelectedBorrow!.BorrowId, ReturnDate!.Value);

            if (result.Success)
            {
                await LoadActiveBorrowingsAsync();
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
