using Microsoft.AspNetCore.Mvc;
using MES.Contracts.Common;
using MES.Infrastructure.Persistence;
using MES.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace MES.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MasterDataController : ControllerBase
{
    private readonly MesDbContext _context;

    public MasterDataController(MesDbContext context)
    {
        _context = context;
    }

    [HttpGet("customers")]
    public async Task<ApiResponse<List<object>>> GetCustomers()
    {
        var items = await _context.MasterCustomers
            .Where(x => x.Status == "Active")
            .OrderBy(x => x.CustomerName)
            .Select(x => new
            {
                x.CustomerId,
                x.CustomerCode,
                x.CustomerName,
                x.QualityLevel,
                x.CustomerPnPrefix
            })
            .ToListAsync();

        return ApiResponse<List<object>>.Ok(items.Cast<object>().ToList());
    }

    [HttpGet("products")]
    public async Task<ApiResponse<List<object>>> GetProducts([FromQuery] string? customerId = null)
    {
        var query = _context.MasterProducts.AsQueryable();

        if (!string.IsNullOrEmpty(customerId))
            query = query.Where(x => x.CustomerId == customerId);

        var items = await query
            .Where(x => x.Status == "Active")
            .OrderBy(x => x.ProductName)
            .Select(x => new
            {
                x.ProductId,
                x.ProductName,
                x.DieName,
                x.PackageType,
                x.ProcessStage,
                x.DefaultRouteId,
                x.CustomerId,
                x.CustomerName
            })
            .ToListAsync();

        return ApiResponse<List<object>>.Ok(items.Cast<object>().ToList());
    }

    [HttpGet("routes")]
    public async Task<ApiResponse<List<object>>> GetRoutes([FromQuery] string? productId = null)
    {
        var query = _context.MasterRoutes.Where(x => x.IsActive);

        if (!string.IsNullOrEmpty(productId))
            query = query.Where(x => x.ProductId == productId);

        var items = await query
            .OrderBy(x => x.RouteName)
            .Select(x => new
            {
                x.RouteId,
                x.RouteName,
                x.RouteVersion,
                x.ProductId,
                x.PackageType
            })
            .ToListAsync();

        return ApiResponse<List<object>>.Ok(items.Cast<object>().ToList());
    }

    [HttpGet("routes/{routeId}/steps")]
    public async Task<ApiResponse<List<object>>> GetRouteSteps(string routeId)
    {
        var steps = await _context.MasterRouteSteps
            .Where(x => x.RouteId == routeId)
            .OrderBy(x => x.StepSeq)
            .Select(x => new
            {
                x.StepSeq,
                x.StepCode,
                x.StepName,
                x.EquipmentGroup
            })
            .ToListAsync();

        return ApiResponse<List<object>>.Ok(steps.Cast<object>().ToList());
    }

    [HttpGet("equipments")]
    public async Task<ApiResponse<List<object>>> GetEquipments([FromQuery] string? group = null, [FromQuery] string? status = null)
    {
        var query = _context.MasterEquipments.AsQueryable();

        if (!string.IsNullOrEmpty(group))
            query = query.Where(x => x.EquipmentGroup == group);

        if (!string.IsNullOrEmpty(status))
            query = query.Where(x => x.Status == status);

        var items = await query
            .OrderBy(x => x.EquipmentName)
            .Select(x => new
            {
                x.EquipmentId,
                x.EquipmentName,
                x.EquipmentGroup,
                x.EquipmentType,
                x.ProcessStage,
                x.Status,
                x.Location
            })
            .ToListAsync();

        return ApiResponse<List<object>>.Ok(items.Cast<object>().ToList());
    }

    [HttpGet("departments")]
    public async Task<ApiResponse<List<object>>> GetDepartments()
    {
        var items = await _context.SysDepartments
            .Where(x => x.Status == "Active")
            .OrderBy(x => x.DeptName)
            .Select(x => new
            {
                x.DeptId,
                x.DeptName,
                x.ParentId
            })
            .ToListAsync();

        return ApiResponse<List<object>>.Ok(items.Cast<object>().ToList());
    }

    [HttpGet("roles")]
    public async Task<ApiResponse<List<object>>> GetRoles()
    {
        var items = await _context.SysRoles
            .OrderBy(x => x.Level)
            .Select(x => new
            {
                x.RoleId,
                x.RoleName,
                x.Level
            })
            .ToListAsync();

        return ApiResponse<List<object>>.Ok(items.Cast<object>().ToList());
    }

    [HttpGet("users")]
    public async Task<ApiResponse<List<object>>> GetUsers([FromQuery] string? deptId = null)
    {
        var query = _context.SysUsers.Where(x => x.IsActive);

        if (!string.IsNullOrEmpty(deptId))
            query = query.Where(x => x.DeptId == deptId);

        var items = await query
            .OrderBy(x => x.UserName)
            .Select(x => new
            {
                x.UserId,
                x.UserName,
                x.RoleId,
                x.DeptId
            })
            .ToListAsync();

        return ApiResponse<List<object>>.Ok(items.Cast<object>().ToList());
    }
}
