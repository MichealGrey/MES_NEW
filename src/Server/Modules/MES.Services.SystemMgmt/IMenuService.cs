using MES.Contracts.SystemMgmt;
using MES.Infrastructure.Persistence;
using MES.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace MES.Services.SystemMgmt;

public interface IMenuService
{
    Task<List<MenuResponse>> GetAllMenusAsync();
    Task<List<MenuTreeNode>> GetUserMenusAsync(string userId);
    Task<MenuResponse> CreateMenuAsync(MenuCreateRequest request);
    Task<bool> UpdateMenuAsync(string menuId, MenuUpdateRequest request);
    Task<bool> DeleteMenuAsync(string menuId);
}
