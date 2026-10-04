using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

        public async Task<IEnumerable<StudentDto>> GetAllStudentsAsync(CancellationToken cancellationToken = default)
        {
            var students = await _studentRepository.GetAllAsync(cancellationToken);
            return students.Select(s => new StudentDto
            {
                StudentID = s.StudentID,
                FullName = s.FullName
            });
        }

        public async Task<IEnumerable<EquipmentDto>> GetAllEquipmentAsync(CancellationToken cancellationToken = default)
        {
            var equipment = await _equipmentRepository.GetAllAsync(cancellationToken);
            return equipment.Select(e => new EquipmentDto
            {
                Id = e.Id,
                EquipmentName = e.EquipmentName
            });
        }

        public async Task<IEnumerable<EquipmentItemDto>> GetEquipmentItemsAsync(CancellationToken cancellationToken = default)
        {
            var equipment = await _equipmentRepository.GetAllAsync(cancellationToken);
            return equipment.Select(e => new EquipmentItemDto
            {
                Id = e.Id,
                EquipmentName = e.EquipmentName,
                EquipmentType = e.EquipmentType,
                IsAvailable = e.IsAvailable
            });
        }
    }
}
