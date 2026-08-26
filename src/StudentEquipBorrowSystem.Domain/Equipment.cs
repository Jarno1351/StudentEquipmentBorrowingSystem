using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class Equipment
    {
        public Guid Id { get; private set; }
        public string EquipmentName { get; private set; }
        public string EquipmentType { get; private set; }

        public bool IsAvailable { get; private set; } = true;

        public Equipment(Guid id, string equipmentName, string equipmentType)
        {
            Id = id;
            EquipmentName = equipmentName;
            EquipmentType = equipmentType;
        }

        public void MarkAsBorrowed()
        {
            IsAvailable = false;
        }

        public void MarkAsReturned()
        {
            IsAvailable = true;
        }
    }


}
