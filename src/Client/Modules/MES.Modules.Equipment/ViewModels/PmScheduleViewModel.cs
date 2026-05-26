using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace MES.Modules.Equipment.ViewModels;

public class PmScheduleItem
{
    public string EquipmentId { get; set; } = string.Empty;
    public string PmType { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public string Status { get; set; } = "Planned";
}

public class PmScheduleViewModel : BindableBase
{
    private ObservableCollection<PmScheduleItem> _pmSchedules = [];
    public ObservableCollection<PmScheduleItem> PmSchedules { get => _pmSchedules; set => SetProperty(ref _pmSchedules, value); }

    public PmScheduleViewModel()
    {
        PmSchedules = new ObservableCollection<PmScheduleItem>
        {
            new() { EquipmentId = "WB-01", PmType = "年度维护", DueDate = DateTime.Now.AddDays(7), Status = "Planned" },
            new() { EquipmentId = "MP-01", PmType = "季度维护", DueDate = DateTime.Now.AddDays(3), Status = "Due" },
            new() { EquipmentId = "TH-01", PmType = "月度维护", DueDate = DateTime.Now.AddDays(-1), Status = "Overdue" },
            new() { EquipmentId = "DB-01", PmType = "周维护", DueDate = DateTime.Now.AddDays(5), Status = "Planned" },
        };
    }
}
