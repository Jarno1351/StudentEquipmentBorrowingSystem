using Applications;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace StudentBorrowEquipSystem.Desktop.ViewModels;

public partial class EquipmentViewModel : ViewModelBase
{
    private readonly ILookupAppService _lookupService;

    public ObservableCollection<EquipmentItemViewModel> Equipment { get; } = new();

    public EquipmentViewModel(ILookupAppService lookupService)
    {
        _lookupService = lookupService;
        LoadEquipmentAsync().Wait();
    }

    public override void OnNavigatedTo() => LoadEquipmentAsync().Wait();

    private async Task LoadEquipmentAsync()
    {
        Equipment.Clear();
        var items = await _lookupService.GetEquipmentItemsAsync();
        foreach (var e in items)
        {
            Equipment.Add(new EquipmentItemViewModel
            {
                Id = e.Id,
                EquipmentName = e.EquipmentName,
                EquipmentType = e.EquipmentType,
                IsAvailable = e.IsAvailable
            });
        }
    }
}
