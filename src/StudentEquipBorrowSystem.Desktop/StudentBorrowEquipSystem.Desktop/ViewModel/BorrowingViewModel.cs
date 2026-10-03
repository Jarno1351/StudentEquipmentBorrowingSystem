using Applications;
using Applications.Dto;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;

namespace StudentBorrowEquipSystem.Desktop.ViewModels;

public partial class BorrowingViewModel : ViewModelBase
{
    private readonly IBorrowAppService _borrowAppService;
    private readonly ILookupAppService _lookupService;

    public ObservableCollection<StudentDto> Students { get; } = new();
    public ObservableCollection<EquipmentDto> Equipment { get; } = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ValidationMessage))]
    [NotifyPropertyChangedFor(nameof(HasValidationMessage))]
    [NotifyCanExecuteChangedFor(nameof(BorrowCommand))]
    private StudentDto? selectedStudent;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ValidationMessage))]
    [NotifyPropertyChangedFor(nameof(HasValidationMessage))]
    [NotifyCanExecuteChangedFor(nameof(BorrowCommand))]
    private EquipmentDto? selectedEquipment;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ValidationMessage))]
    [NotifyPropertyChangedFor(nameof(HasValidationMessage))]
    [NotifyCanExecuteChangedFor(nameof(BorrowCommand))]
    private DateTime? dueDate;

    // Presentation/input validation only. Business rules (limit, availability) stay in Application/Domain.
    public string? ValidationMessage =>
        SelectedStudent == null ? "Please select a student." :
        SelectedEquipment == null ? "Please select equipment." :
        DueDate == null ? "Please select a due date." :
        DueDate.Value.Date < DateTime.Today ? "Due date cannot be in the past." :
        null;

    public bool HasValidationMessage => ValidationMessage != null;

    public BorrowingViewModel(IBorrowAppService borrowAppService, ILookupAppService lookupService)
    {
        _borrowAppService = borrowAppService;
        _lookupService = lookupService;
        LoadStudents();
        LoadEquipment();
    }

    public override void OnNavigatedTo()
    {
        LoadStudents();
        LoadEquipment();
    }

    private void LoadStudents()
    {
        var keep = SelectedStudent?.StudentID;
        Students.Clear();
        foreach (var s in _lookupService.GetAllStudents())
            Students.Add(s);
        if (keep != null)
            foreach (var s in Students) if (s.StudentID == keep) SelectedStudent = s;
    }

    private void LoadEquipment()
    {
        Equipment.Clear();
        foreach (var e in _lookupService.GetAllEquipment())
            Equipment.Add(e);
    }

    private bool CanBorrow() => ValidationMessage == null;

    [RelayCommand(CanExecute = nameof(CanBorrow))]
    private void Borrow()
    {
        var problem = ValidationMessage;
        if (problem != null) { ShowError(problem); return; }

        try
        {
            var result = _borrowAppService.BorrowEquipment(
                SelectedStudent!.StudentID, SelectedEquipment!.Id, DueDate!.Value);

            if (result.Success)
            {
                LoadEquipment();
                SelectedEquipment = null;
                DueDate = null;
                ShowSuccess("Equipment borrowed successfully.");
            }
            else
            {
                ShowError(result.Message ?? "Borrow failed.");
            }
        }
        catch (Exception ex)
        {
            ShowError($"Unexpected error: {ex.Message}");
        }
    }
}
