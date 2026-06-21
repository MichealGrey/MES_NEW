namespace MES.Modules.Yield.Models;

public class WaferDieItem
{
    public string WaferId { get; set; } = string.Empty;
    public int DieX { get; set; }
    public int DieY { get; set; }
    public string BinCode { get; set; } = "1";
    public string Result { get; set; } = "Good";
    public string DefectType { get; set; } = string.Empty;
}
