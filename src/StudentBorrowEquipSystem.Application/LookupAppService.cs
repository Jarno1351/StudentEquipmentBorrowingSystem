using System.Collections.Generic;
using System.Linq;
using Applications.Dto;
using Domain;

namespace Applications
{
    public class LookupAppService : ILookupAppService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IEquipmentRepository _equipmentRepository;

        public LookupAppService(IStudentRepository studentRepository, IEquipmentRepository equipmentRepository)
        {
            _studentRepository = studentRepository;
            _equipmentRepository = equipmentRepository;
        }

        public IEnumerable<StudentDto> GetAllStudents()
        {
            return _studentRepository.GetAll().Select(s => new StudentDto
            {
                StudentID = s.StudentID,
                FullName = s.FullName
            });
        }

        public IEnumerable<EquipmentDto> GetAllEquipment()
        {
            return _equipmentRepository.GetAll().Select(e => new EquipmentDto
            {
                Id = e.Id,
                EquipmentName = e.EquipmentName
            });
        }
    }
}
