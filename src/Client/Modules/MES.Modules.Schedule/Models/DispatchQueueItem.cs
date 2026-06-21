namespace MES.Modules.Schedule.Models;

public class DispatchQueueItem
{
    public string EquipmentId { get; set; } = string.Empty;
    public string LotId { get; set; } = string.Empty;
    public string Product { get; set; } = string.Empty;
    public int Priority { get; set; }
    public string Step { get; set; } = string.Empty;
    public string RecipeId { get; set; } = string.Empty;
    public string Customer { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
}
