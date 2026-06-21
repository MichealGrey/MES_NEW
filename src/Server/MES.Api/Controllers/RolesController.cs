using Microsoft.AspNetCore.Mvc;
using MES.Contracts.Common;
using MES.Contracts.SystemMgmt;
using MES.Services.SystemMgmt;

namespace MES.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RolesController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpGet]
    public async Task<ApiResponse<List<RoleResponse>>> GetRoles()
    {
        var result = await _roleService.GetRolesAsync();
        return ApiResponse<List<RoleResponse>>.Ok(result);
    }

    [HttpGet("{roleId}")]
    public async Task<ApiResponse<RoleResponse?>> GetRole(string roleId)
    {
        var result = await _roleService.GetRoleAsync(roleId);
        return result != null
            ? ApiResponse<RoleResponse?>.Ok(result)
            : ApiResponse<RoleResponse?>.Fail("Role not found");
    }

    [HttpPost]
    public async Task<ApiResponse<RoleResponse>> CreateRole([FromBody] RoleCreateRequest request)
    {
        var result = await _roleService.CreateRoleAsync(request);
        return ApiResponse<RoleResponse>.Ok(result, "Role created");
    }

    [HttpPut("{roleId}")]
    public async Task<ApiResponse<bool>> UpdateRole(string roleId, [FromBody] RoleUpdateRequest request)
    {
        var result = await _roleService.UpdateRoleAsync(roleId, request);
        return result
            ? ApiResponse<bool>.Ok(true, "Role updated")
            : ApiResponse<bool>.Fail("Role not found");
    }

    [HttpDelete("{roleId}")]
    public async Task<ApiResponse<bool>> DeleteRole(string roleId)
    {
        var result = await _roleService.DeleteRoleAsync(roleId);
        return result
            ? ApiResponse<bool>.Ok(true, "Role deleted")
            : ApiResponse<bool>.Fail("Role not found or has assigned users");
    }

    [HttpPost("{roleId}/menus")]
    public async Task<ApiResponse<bool>> AssignMenus(string roleId, [FromBody] List<string> menuIds)
    {
        var result = await _roleService.AssignMenusAsync(roleId, menuIds);
        return result
            ? ApiResponse<bool>.Ok(true, "Menus assigned")
            : ApiResponse<bool>.Fail("Role not found");
    }

    [HttpPost("{roleId}/permissions")]
    public async Task<ApiResponse<bool>> AssignPermissions(string roleId, [FromBody] List<string> permissionCodes)
    {
        var result = await _roleService.AssignPermissionsAsync(roleId, permissionCodes);
        return result
            ? ApiResponse<bool>.Ok(true, "Permissions assigned")
            : ApiResponse<bool>.Fail("Role not found");
    }
}
