using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Applications.Dto;

namespace Applications
{
    public interface ILookupAppService
    {
        Task<IEnumerable<StudentDto>> GetAllStudentsAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<EquipmentDto>> GetAllEquipmentAsync(CancellationToken cancellationToken = default);

        /// Equipment that can be borrowed right now (for the borrow form).
        Task<IEnumerable<EquipmentDto>> GetAvailableEquipmentAsync(CancellationToken cancellationToken = default);

        Task<IEnumerable<EquipmentItemDto>> GetEquipmentItemsAsync(CancellationToken cancellationToken = default);
    }
}