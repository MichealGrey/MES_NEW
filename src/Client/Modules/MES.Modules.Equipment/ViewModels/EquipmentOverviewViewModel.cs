using Prism.Mvvm;
using MES.Modules.Equipment.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace MES.Modules.Equipment.ViewModels;

public class EquipmentOverviewViewModel : BindableBase
{
    private ObservableCollection<EquipmentInfo> _equipments = [];
    public ObservableCollection<EquipmentInfo> Equipments { get => _equipments; set => SetProperty(ref _equipments, value); }

    public EquipmentOverviewViewModel()
    {
        var types = new[] { "DicingSaw", "DieBonder", "WireBonder", "MoldingPress", "LaserMark", "TrimForm", "TestHandler", "BurnIn", "AOI", "XRay", "PlasmaClean", "TapeReel" };
        var names = new[] { "切割机", "贴片机", "焊线机", "塑封机", "激光打标机", "切筋成型机", "测试机", "老化炉", "AOI检测机", "X-Ray检测机", "等离子清洗机", "编带机" };
        var typePrefixes = new[] { "WS", "DB", "WB", "MP", "MK", "TF", "TH", "BI", "AOI", "XR", "PC", "TR" };
        var areas = new[] { "PKG1", "PKG2", "TEST1", "TEST2" };
        var statuses = new[] { "Idle", "Processing", "Down", "PM", "Engineering" };

        Equipments = new ObservableCollection<EquipmentInfo>(
            Enumerable.Range(1, 30).Select(i => new EquipmentInfo
            {
                EquipmentId = $"{typePrefixes[(i - 1) % 12]}-{(i - 1) / 12 + 1:D2}",
                Name = $"{names[(i - 1) % 12]}-{(i - 1) / 12 + 1}",
                Type = types[(i - 1) % 12],
                Area = areas[(i - 1) % 4],
                Status = statuses[i % 5],
                CurrentLot = statuses[i % 5] == "Processing" ? $"LOT-{2026000 + i}" : null,
                RecipeId = statuses[i % 5] == "Processing" ? $"RCP-{i % 10:D3}" : null
            }));
    }
}
