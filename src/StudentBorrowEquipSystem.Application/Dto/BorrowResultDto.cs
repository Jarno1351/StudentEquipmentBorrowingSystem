using System;

namespace Applications.Dto;

public sealed class BorrowResultDto
{
    public bool Success { get; init; }
    public string? Message { get; init; }
    public Guid? BorrowId { get; init; }
    public DateTime? DueDate { get; init; }
}
