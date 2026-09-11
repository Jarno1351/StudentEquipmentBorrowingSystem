using Applications;
using Applications.Dto;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;

namespace StudentBorrowEquipSystem.Desktop.ViewModels;

public partial class BorrowingViewModel : ObservableObject
{
    private readonly IBorrowAppService _borrowAppService;
    private readonly ILookupAppService _lookupService;

    public ObservableCollection<StudentDto> Students { get; } = new();
    public ObservableCollection<EquipmentDto> Equipment { get; } = new();

    [ObservableProperty]
    private StudentDto? selectedStudent;

    [ObservableProperty]
    private EquipmentDto? selectedEquipment;

    [ObservableProperty]
    private DateTime? dueDate;

    [ObservableProperty]
    private string? statusMessage;

    public BorrowingViewModel(
        IBorrowAppService borrowAppService,
        ILookupAppService lookupService)
    {
        _borrowAppService = borrowAppService;
        _lookupService = lookupService;

        LoadStudents();
        LoadEquipment();
    }

    private void LoadStudents()
    {
        Students.Clear();
        foreach (var s in _lookupService.GetAllStudents())
            Students.Add(s);
    }

    private void LoadEquipment()
    {
        Equipment.Clear();
        foreach (var e in _lookupService.GetAllEquipment())
            Equipment.Add(e);
    }

    [RelayCommand]
    private void Borrow()
    {
        if (SelectedStudent == null)
        {
            StatusMessage = "Please select a student.";
            return;
        }

        if (SelectedEquipment == null)
        {
            StatusMessage = "Please select equipment.";
            return;
        }

        if (DueDate == null)
        {
            StatusMessage = "Please select an expected return date.";
            return;
        }

        var result = _borrowAppService.BorrowEquipment(SelectedStudent.StudentID, SelectedEquipment.Id, DueDate.Value);

        if (result.Success)
        {
            StatusMessage = $"Successfully borrowed {SelectedEquipment.EquipmentName}. Due: {result.DueDate:yyyy-MM-dd}";
            LoadEquipment();
        }
        else
        {
            StatusMessage = result.Message ?? "Borrow failed.";
        }
    }
}
