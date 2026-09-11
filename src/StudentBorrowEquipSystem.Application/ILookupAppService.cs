using System.Collections.Generic;
using Applications.Dto;

namespace Applications
{
    public interface ILookupAppService
    {
        IEnumerable<StudentDto> GetAllStudents();
        IEnumerable<EquipmentDto> GetAllEquipment();
    }
}
