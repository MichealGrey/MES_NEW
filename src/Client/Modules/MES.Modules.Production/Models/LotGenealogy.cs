namespace MES.Modules.Production.Models;

public class LotGenealogy
{
    public string GenealogyId { get; set; } = Guid.NewGuid().ToString("N");
    public string ParentLotId { get; set; } = string.Empty;
    public string ChildLotId { get; set; } = string.Empty;
    public string RelationType { get; set; } = string.Empty;
    public string StepCode { get; set; } = string.Empty;
    public int StepSeq { get; set; }
    public int Qty { get; set; }
    public string? Grade { get; set; }
    public string? WaferLotId { get; set; }
    public string OperatorId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? ReasonCode { get; set; }
    public string? Remark { get; set; }
}
