namespace MES.Modules.Trace.Models;

public class GenealogyItem
{
    public string LotId { get; set; } = string.Empty;
    public string ParentLotId { get; set; } = string.Empty;
    public string Product { get; set; } = string.Empty;
    public string Operation { get; set; } = string.Empty;
    public DateTime Time { get; set; }
    public int StripCount { get; set; }
}
