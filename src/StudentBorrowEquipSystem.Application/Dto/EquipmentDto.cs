using System;

namespace Applications.Dto;

public sealed class EquipmentDto
{
    public Guid Id { get; init; }
    public string EquipmentName { get; init; } = string.Empty;
}
