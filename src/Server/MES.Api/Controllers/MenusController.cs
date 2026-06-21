using Microsoft.AspNetCore.Mvc;
using MES.Contracts.Common;
using MES.Contracts.SystemMgmt;
using MES.Services.SystemMgmt;

namespace MES.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MenusController : ControllerBase
{
    private readonly IMenuService _menuService;

    public MenusController(IMenuService menuService)
    {
        _menuService = menuService;
    }

    [HttpGet]
    public async Task<ApiResponse<List<MenuResponse>>> GetAllMenus()
    {
        var result = await _menuService.GetAllMenusAsync();
        return ApiResponse<List<MenuResponse>>.Ok(result);
    }

    [HttpGet("user/{userId}")]
    public async Task<ApiResponse<List<MenuTreeNode>>> GetUserMenus(string userId)
    {
        var result = await _menuService.GetUserMenusAsync(userId);
        return ApiResponse<List<MenuTreeNode>>.Ok(result);
    }

    [HttpPost]
    public async Task<ApiResponse<MenuResponse>> CreateMenu([FromBody] MenuCreateRequest request)
    {
        var result = await _menuService.CreateMenuAsync(request);
        return ApiResponse<MenuResponse>.Ok(result, "Menu created");
    }

    [HttpPut("{menuId}")]
    public async Task<ApiResponse<bool>> UpdateMenu(string menuId, [FromBody] MenuUpdateRequest request)
    {
        var result = await _menuService.UpdateMenuAsync(menuId, request);
        return result
            ? ApiResponse<bool>.Ok(true, "Menu updated")
            : ApiResponse<bool>.Fail("Menu not found");
    }

    [HttpDelete("{menuId}")]
    public async Task<ApiResponse<bool>> DeleteMenu(string menuId)
    {
        var result = await _menuService.DeleteMenuAsync(menuId);
        return result
            ? ApiResponse<bool>.Ok(true, "Menu deleted")
            : ApiResponse<bool>.Fail("Menu not found or has child menus");
    }
}
