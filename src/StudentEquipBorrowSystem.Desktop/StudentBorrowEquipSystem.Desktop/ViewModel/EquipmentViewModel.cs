using Applications;
using System.Collections.ObjectModel;

namespace StudentBorrowEquipSystem.Desktop.ViewModels;

public partial class EquipmentViewModel : ViewModelBase
{
    private readonly ILookupAppService _lookupService;

    public ObservableCollection<EquipmentItemViewModel> Equipment { get; } = new();

    public EquipmentViewModel(ILookupAppService lookupService)
    {
        _lookupService = lookupService;
        LoadEquipment();
    }

    public override void OnNavigatedTo() => LoadEquipment();

    private void LoadEquipment()
    {
        Equipment.Clear();
        foreach (var e in _lookupService.GetEquipmentItems())
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
