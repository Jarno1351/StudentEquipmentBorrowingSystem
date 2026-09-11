using Applications;
using Applications.Dto;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;

namespace StudentBorrowEquipSystem.Desktop.ViewModels;

public partial class ActiveBorrowingsViewModel : ObservableObject
{
    private readonly IBorrowAppService _borrowAppService;

    public ObservableCollection<BorrowDto> ActiveBorrowings { get; } = new();

    [ObservableProperty]
    private BorrowDto? selectedBorrow;

    [ObservableProperty]
    private DateTime? returnDate;

    [ObservableProperty]
    private string? statusMessage;

    public ActiveBorrowingsViewModel(IBorrowAppService borrowAppService)
    {
        _borrowAppService = borrowAppService;
        LoadActiveBorrowings();
        ReturnDate = DateTime.Now;
    }

    public void LoadActiveBorrowings()
    {
        ActiveBorrowings.Clear();
        foreach (var b in _borrowAppService.GetActiveBorrowings())
            ActiveBorrowings.Add(b);
    }

    [RelayCommand]
    private void ReturnSelected()
    {
        if (SelectedBorrow == null)
        {
            StatusMessage = "Please select a borrowing record.";
            return;
        }

        var date = ReturnDate ?? DateTime.Now;

        var result = _borrowAppService.ReturnEquipment(SelectedBorrow.BorrowId, date);

        if (result.Success)
        {
            StatusMessage = "Equipment returned successfully.";
            LoadActiveBorrowings();
        }
        else
        {
            StatusMessage = result.Message ?? "Return failed.";
        }
    }
}
