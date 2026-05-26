namespace MES.Modules.Yield.Models;

public class WaferDieItem
{
    public int Row { get; set; }
    public int Col { get; set; }
    public string DieId { get; set; } = string.Empty;
    public string Result { get; set; } = "Pass";
}
