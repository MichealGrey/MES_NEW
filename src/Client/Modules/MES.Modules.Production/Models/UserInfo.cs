namespace MES.Modules.Production.Models;

public class UserInfo
{
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Role { get; set; } = "Operator"; // Operator/Engineer/QA/Supervisor/Admin
    public string Department { get; set; } = string.Empty;
    public string Shift { get; set; } = string.Empty; // A/B/C
    public bool IsActive { get; set; } = true;
    public List<string> Permissions { get; set; } = [];
    public List<string> AuthorizedRoutes { get; set; } = [];
    public List<string> AuthorizedEquipments { get; set; } = [];
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
