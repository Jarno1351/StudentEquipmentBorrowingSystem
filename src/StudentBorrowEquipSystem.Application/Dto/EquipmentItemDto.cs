using System;

namespace Applications.Dto
{
    /// Read model for the Equipment catalogue screen (name, type, current availability).
    public class EquipmentItemDto
    {
        public Guid Id { get; set; }
        public string EquipmentName { get; set; } = string.Empty;
        public string EquipmentType { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }
    }
}
