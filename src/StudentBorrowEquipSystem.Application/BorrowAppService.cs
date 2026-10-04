using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

        public async Task<BorrowResultDto> BorrowEquipmentAsync(string studentId, Guid equipmentId, DateTime dueDate, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(studentId))
                return new BorrowResultDto { Success = false, Message = "Student id is required." };

            var student = await _studentRepository.GetByIdAsync(studentId, cancellationToken);
            if (student == null)
                return new BorrowResultDto { Success = false, Message = $"Student '{studentId}' not found." };

            var equipment = await _equipmentRepository.GetByIdAsync(equipmentId, cancellationToken);
            if (equipment == null)
                return new BorrowResultDto { Success = false, Message = $"Equipment '{equipmentId}' not found." };

            try
            {
                var borrow = await _borrowService.BorrowEquipmentAsync(student, equipment, dueDate, cancellationToken);
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

        public async Task<BorrowResultDto> ReturnEquipmentAsync(Guid borrowId, DateTime returnDate, CancellationToken cancellationToken = default)
        {
            try
            {
                await _borrowService.ReturnEquipmentAsync(borrowId, returnDate, cancellationToken);
                return new BorrowResultDto { Success = true };
            }
            catch (Exception ex)
            {
                return new BorrowResultDto { Success = false, Message = ex.Message };
            }
        }

        public async Task<IEnumerable<BorrowDto>> GetActiveBorrowingsAsync(CancellationToken cancellationToken = default)
        {
            // The repository filters and joins in the database; this method only maps to DTOs.
            var active = await _borrowRepository.GetActiveAsync(cancellationToken);

            return active.Select(b => new BorrowDto
            {
                BorrowId = b.Id,
                StudentId = b.StudentBorrower.StudentID,
                StudentName = b.StudentBorrower.FullName,
                EquipmentId = b.EquipmentBorrowed.Id,
                EquipmentName = b.EquipmentBorrowed.EquipmentName,
                BorrowDate = b.BorrowDate,
                DueDate = b.DueDate,
                Status = b.Status.ToString()
            }).ToList();
        }
    }
}