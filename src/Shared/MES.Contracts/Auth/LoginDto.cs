using MES.Contracts.System;

namespace MES.Contracts.Auth;

public class LoginRequest
{
    public string UserId { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string RoleId { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public string DeptId { get; set; } = string.Empty;
    public List<string> Permissions { get; set; } = [];
    public List<MenuDto> Menus { get; set; } = [];
}

public class LoginLogDto
{
    public long Id { get; set; }
    public string? UserId { get; set; }
    public string? EmployeeId { get; set; }
    public DateTime? LoginTime { get; set; }
    public string? IpAddress { get; set; }
    public string? Result { get; set; }
    public string? ErrorMessage { get; set; }
}
