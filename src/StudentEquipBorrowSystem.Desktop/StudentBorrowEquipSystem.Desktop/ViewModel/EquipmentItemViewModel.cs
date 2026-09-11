using System;

namespace StudentBorrowEquipSystem.Desktop.ViewModels;

public class EquipmentItemViewModel
{
    public Guid Id { get; set; }
    public string EquipmentName { get; set; } = string.Empty;
    public string EquipmentType { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
}
