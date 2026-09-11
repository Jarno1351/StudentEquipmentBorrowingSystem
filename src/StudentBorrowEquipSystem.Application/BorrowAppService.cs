using System;
using Applications.Dto;
using Domain;

namespace Applications
{
  
    public class BorrowAppService : IBorrowAppService
    {
        private readonly IBorrowService _borrowService;
        private readonly IStudentRepository _studentRepository;
        private readonly IEquipmentRepository _equipmentRepository;

        public BorrowAppService(
            IBorrowService borrowService,
            IStudentRepository studentRepository,
            IEquipmentRepository equipmentRepository)
        {
            _borrowService = borrowService ?? throw new ArgumentNullException(nameof(borrowService));
            _studentRepository = studentRepository ?? throw new ArgumentNullException(nameof(studentRepository));
            _equipmentRepository = equipmentRepository ?? throw new ArgumentNullException(nameof(equipmentRepository));
        }

        public BorrowResultDto BorrowEquipment(string studentId, Guid equipmentId, DateTime dueDate)
        {
            if (string.IsNullOrWhiteSpace(studentId))
                return new BorrowResultDto { Success = false, Message = "Student id is required." };

            var student = _studentRepository.GetById(studentId);
            if (student == null)
                return new BorrowResultDto { Success = false, Message = $"Student '{studentId}' not found." };

            var equipment = _equipmentRepository.GetById(equipmentId);
            if (equipment == null)
                return new BorrowResultDto { Success = false, Message = $"Equipment '{equipmentId}' not found." };

            try
            {
                var borrow = _borrowService.BorrowEquipment(student, equipment, dueDate);
                return new BorrowResultDto
                {
                    Success = true,
                    BorrowId = borrow.Id,
                    DueDate = borrow.DueDate
                };
            }
            catch (Exception ex)
            {
                return new BorrowResultDto { Success = false, Message = ex.Message };
            }
        }
    }
}
