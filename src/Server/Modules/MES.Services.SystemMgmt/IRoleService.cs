using MES.Contracts.SystemMgmt;

namespace MES.Services.SystemMgmt;

public interface IRoleService
{
    Task<List<RoleResponse>> GetRolesAsync();
    Task<RoleResponse?> GetRoleAsync(string roleId);
    Task<RoleResponse> CreateRoleAsync(RoleCreateRequest request);
    Task<bool> UpdateRoleAsync(string roleId, RoleUpdateRequest request);
    Task<bool> DeleteRoleAsync(string roleId);
    Task<bool> AssignMenusAsync(string roleId, List<string> menuIds);
    Task<bool> AssignPermissionsAsync(string roleId, List<string> permissionCodes);
}
