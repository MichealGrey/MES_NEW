namespace MES.Modules.Quality.Models;

public class InspectionItem
{
    public string InspectionId { get; set; } = string.Empty;
    public string LotId { get; set; } = string.Empty;
    public string Product { get; set; } = string.Empty;
    public string InspectionType { get; set; } = string.Empty;
    public string Result { get; set; } = string.Empty;
    public string Inspector { get; set; } = string.Empty;
    public DateTime InspectTime { get; set; }
}
