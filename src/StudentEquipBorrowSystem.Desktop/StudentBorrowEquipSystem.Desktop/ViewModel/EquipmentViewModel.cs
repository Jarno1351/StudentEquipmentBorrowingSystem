using Applications;
using CommunityToolkit.Mvvm.ComponentModel;
using Domain;
using System.Collections.ObjectModel;

namespace StudentBorrowEquipSystem.Desktop.ViewModels;


public partial class EquipmentViewModel : ObservableObject
{
    private readonly IEquipmentRepository _equipmentRepository;

    public ObservableCollection<EquipmentItemViewModel> Equipment { get; } = new();

    public EquipmentViewModel(IEquipmentRepository equipmentRepository)
    {
        _equipmentRepository = equipmentRepository;

        LoadEquipment();
    }

    private void LoadEquipment()
    {
        Equipment.Clear();

        foreach (var equipment in _equipmentRepository.GetAll())
        {
            Equipment.Add(new EquipmentItemViewModel
            {
                Id = equipment.Id,
                EquipmentName = equipment.EquipmentName,
                EquipmentType = equipment.EquipmentType,
                IsAvailable = equipment.IsAvailable
            });
        }
    }
}