namespace MES.Modules.Trace.Models;

public class GenealogyItem
{
    public string Id { get; set; } = string.Empty;
    public string LotId { get; set; } = string.Empty;
    public string ParentLotId { get; set; } = string.Empty;
    public string ChildLotId { get; set; } = string.Empty;
    public string Product { get; set; } = string.Empty;
    public string Operation { get; set; } = string.Empty;
    public DateTime Time { get; set; }
    public string EquipmentId { get; set; } = string.Empty;
    public string Operator { get; set; } = string.Empty;
}
