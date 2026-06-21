using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MES.Contracts.Common;
using MES.Contracts.Equipment;
using MES.Services.Equipment;

namespace MES.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EquipmentController : ControllerBase
{
    private readonly IEquipmentService _equipmentService;
    private readonly ILogger<EquipmentController> _logger;

    public EquipmentController(IEquipmentService equipmentService, ILogger<EquipmentController> logger)
    {
        _equipmentService = equipmentService;
        _logger = logger;
    }

    /// <summary>
    /// 设备列表（分页）
    /// GET /api/equipment
    /// </summary>
    [HttpGet]
    public async Task<ApiResponse<PagedResult<EquipmentListResponse>>> GetEquipment(
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? type = null,
        [FromQuery] string? status = null,
        [FromQuery] string? location = null)
    {
        if (pageIndex < 1) pageIndex = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 20;

        var result = await _equipmentService.GetEquipmentAsync(pageIndex, pageSize, type, status, location);
        return ApiResponse<PagedResult<EquipmentListResponse>>.Ok(result);
    }

    /// <summary>
    /// 设备详情
    /// GET /api/equipment/{id}
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ApiResponse<EquipmentDetailResponse?>> GetEquipmentDetail(string id)
    {
        var result = await _equipmentService.GetEquipmentDetailAsync(id);
        return result != null
            ? ApiResponse<EquipmentDetailResponse?>.Ok(result)
            : ApiResponse<EquipmentDetailResponse?>.Fail("设备不存在");
    }

    /// <summary>
    /// 更新设备状态
    /// PUT /api/equipment/{id}/status
    /// </summary>
    [HttpPut("{id}/status")]
    public async Task<ApiResponse<bool>> UpdateEquipmentStatus(string id, [FromBody] EquipmentStatusUpdateRequest request)
    {
        if (string.IsNullOrWhiteSpace(id))
            return ApiResponse<bool>.Fail("设备ID不能为空");

        request.EquipmentId = id;

        try
        {
            var result = await _equipmentService.UpdateEquipmentStatusAsync(request);
            return result
                ? ApiResponse<bool>.Ok(true, "设备状态已更新")
                : ApiResponse<bool>.Fail("设备不存在");
        }
        catch (ArgumentException ex)
        {
            return ApiResponse<bool>.Fail(ex.Message);
        }
    }

    /// <summary>
    /// 创建维护记录
    /// POST /api/equipment/maintenance
    /// </summary>
    [HttpPost("maintenance")]
    public async Task<ApiResponse<MaintenanceResponse>> CreateMaintenance([FromBody] MaintenanceCreateRequest request)
    {
        try
        {
            var result = await _equipmentService.CreateMaintenanceAsync(request);
            return ApiResponse<MaintenanceResponse>.Ok(result, "维护记录已创建");
        }
        catch (ArgumentException ex)
        {
            return ApiResponse<MaintenanceResponse>.Fail(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return ApiResponse<MaintenanceResponse>.Fail(ex.Message);
        }
    }

    /// <summary>
    /// 查询维护记录
    /// GET /api/equipment/{id}/maintenance
    /// </summary>
    [HttpGet("{id}/maintenance")]
    public async Task<ApiResponse<List<MaintenanceResponse>>> GetMaintenance(
        string id,
        [FromQuery] string? status = null)
    {
        var result = await _equipmentService.GetMaintenanceAsync(id, status);
        return ApiResponse<List<MaintenanceResponse>>.Ok(result);
    }

    /// <summary>
    /// 完成维护
    /// PUT /api/equipment/maintenance/{id}/complete
    /// </summary>
    [HttpPut("maintenance/{id}/complete")]
    public async Task<ApiResponse<bool>> CompleteMaintenance(
        long id,
        [FromBody] CompleteMaintenanceRequest request)
    {
        if (request.ActualHours <= 0)
            return ApiResponse<bool>.Fail("实际工时必须大于0");

        var result = await _equipmentService.CompleteMaintenanceAsync(
            id, request.ActualHours, request.Notes, request.TechnicianId);

        return result
            ? ApiResponse<bool>.Ok(true, "维护记录已完成")
            : ApiResponse<bool>.Fail("维护记录不存在或已完成");
    }

    /// <summary>
    /// 创建故障记录
    /// POST /api/equipment/failures
    /// </summary>
    [HttpPost("failures")]
    public async Task<ApiResponse<FailureResponse>> CreateFailure([FromBody] FailureCreateRequest request)
    {
        try
        {
            var result = await _equipmentService.CreateFailureAsync(request);
            return ApiResponse<FailureResponse>.Ok(result, "故障记录已创建");
        }
        catch (ArgumentException ex)
        {
            return ApiResponse<FailureResponse>.Fail(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return ApiResponse<FailureResponse>.Fail(ex.Message);
        }
    }

    /// <summary>
    /// 查询故障记录
    /// GET /api/equipment/{id}/failures
    /// </summary>
    [HttpGet("{id}/failures")]
    public async Task<ApiResponse<List<FailureResponse>>> GetFailures(
        string id,
        [FromQuery] string? status = null,
        [FromQuery] int? days = null)
    {
        var result = await _equipmentService.GetFailuresAsync(id, status, days);
        return ApiResponse<List<FailureResponse>>.Ok(result);
    }

    /// <summary>
    /// 解决故障
    /// PUT /api/equipment/failures/{id}/resolve
    /// </summary>
    [HttpPut("failures/{id}/resolve")]
    public async Task<ApiResponse<bool>> ResolveFailure(
        long id,
        [FromBody] ResolveFailureRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Resolution))
            return ApiResponse<bool>.Fail("解决说明不能为空");

        var result = await _equipmentService.ResolveFailureAsync(id, request.Resolution, request.ResolvedBy);

        return result
            ? ApiResponse<bool>.Ok(true, "故障已解决")
            : ApiResponse<bool>.Fail("故障记录不存在或已解决");
    }

    /// <summary>
    /// 计算设备OEE
    /// GET /api/equipment/{id}/oee
    /// </summary>
    [HttpGet("{id}/oee")]
    public async Task<ApiResponse<EquipmentOeeResponse>> GetEquipmentOee(
        string id,
        [FromQuery] int days = 30)
    {
        try
        {
            var result = await _equipmentService.GetEquipmentOeeAsync(id, days);
            return ApiResponse<EquipmentOeeResponse>.Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return ApiResponse<EquipmentOeeResponse>.Fail(ex.Message);
        }
    }

    /// <summary>
    /// 设备看板（状态分布、告警）
    /// GET /api/equipment/dashboard
    /// </summary>
    [HttpGet("dashboard")]
    public async Task<ApiResponse<EquipmentDashboardResponse>> GetDashboard()
    {
        var result = await _equipmentService.GetDashboardAsync();
        return ApiResponse<EquipmentDashboardResponse>.Ok(result);
    }
}

/// <summary>
/// 完成维护请求
/// </summary>
public class CompleteMaintenanceRequest
{
    public double ActualHours { get; set; }
    public string? Notes { get; set; }
    public string? TechnicianId { get; set; }
}

/// <summary>
/// 解决故障请求
/// </summary>
public class ResolveFailureRequest
{
    public string Resolution { get; set; } = string.Empty;
    public string? ResolvedBy { get; set; }
}
