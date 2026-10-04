using Applications;
using Applications.Dto;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

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
        LoadStudentsAsync().Wait();
        LoadEquipmentAsync().Wait();
    }

    public override void OnNavigatedTo()
    {
        LoadStudentsAsync().Wait();
        LoadEquipmentAsync().Wait();
    }

    private async Task LoadStudentsAsync()
    {
        var keep = SelectedStudent?.StudentID;
        Students.Clear();
        var students = await _lookupService.GetAllStudentsAsync();
        foreach (var s in students)
            Students.Add(s);
        if (keep != null)
            foreach (var s in Students) if (s.StudentID == keep) SelectedStudent = s;
    }

    private async Task LoadEquipmentAsync()
    {
        Equipment.Clear();
        var equipment = await _lookupService.GetAllEquipmentAsync();
        foreach (var e in equipment)
            Equipment.Add(e);
    }

    private bool CanBorrow() => ValidationMessage == null;

    [RelayCommand(CanExecute = nameof(CanBorrow))]
    private async Task Borrow()
    {
        var problem = ValidationMessage;
        if (problem != null) { ShowError(problem); return; }

        try
        {
            var result = await _borrowAppService.BorrowEquipmentAsync(
                SelectedStudent!.StudentID, SelectedEquipment!.Id, DueDate!.Value);

            if (result.Success)
            {
                await LoadEquipmentAsync();
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
