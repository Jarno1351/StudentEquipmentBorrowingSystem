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
        private readonly IBorrowRepository _borrowRepository;

        public BorrowAppService(IBorrowService borrowService,
                                 IStudentRepository studentRepository,
                                 IEquipmentRepository equipmentRepository,
                                 IBorrowRepository borrowRepository)
        {
            if (borrowService == null) throw new ArgumentNullException(nameof(borrowService));
            if (studentRepository == null) throw new ArgumentNullException(nameof(studentRepository));
            if (equipmentRepository == null) throw new ArgumentNullException(nameof(equipmentRepository));
            if (borrowRepository == null) throw new ArgumentNullException(nameof(borrowRepository));

            _borrowService = borrowService;
            _studentRepository = studentRepository;
            _equipmentRepository = equipmentRepository;
            _borrowRepository = borrowRepository;
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

        public BorrowResultDto ReturnEquipment(Guid borrowId, DateTime returnDate)
        {
            try
            {
                _borrowService.ReturnEquipment(borrowId, returnDate);
                return new BorrowResultDto { Success = true };
            }
            catch (Exception ex)
            {
                return new BorrowResultDto { Success = false, Message = ex.Message };
            }
        }

        public System.Collections.Generic.IEnumerable<BorrowDto> GetActiveBorrowings()
        {
            var now = DateTime.Now;
            var active = _borrowRepository.GetAll();
            var list = new System.Collections.Generic.List<BorrowDto>();
            foreach (var b in active)
            {
                if (b.Status.ToString() == "Active")
                {
                    list.Add(new BorrowDto
                    {
                        BorrowId = b.Id,
                        StudentId = b.StudentBorrower.StudentID,
                        StudentName = b.StudentBorrower.FullName,
                        EquipmentId = b.EquipmentBorrowed.Id,
                        EquipmentName = b.EquipmentBorrowed.EquipmentName,
                        BorrowDate = b.BorrowDate,
                        DueDate = b.DueDate,
                        Status = b.Status.ToString()
                    });
                }
            }

            return list;
        }
    }
}
