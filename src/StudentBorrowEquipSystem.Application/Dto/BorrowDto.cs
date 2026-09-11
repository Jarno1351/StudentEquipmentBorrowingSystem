using System;

namespace Applications.Dto;

public sealed class BorrowDto
{
    public Guid BorrowId { get; init; }
    public string StudentId { get; init; } = string.Empty;
    public string StudentName { get; init; } = string.Empty;
    public Guid EquipmentId { get; init; }
    public string EquipmentName { get; init; } = string.Empty;
    public DateTime BorrowDate { get; init; }
    public DateTime DueDate { get; init; }
    public string Status { get; init; } = string.Empty;
}
