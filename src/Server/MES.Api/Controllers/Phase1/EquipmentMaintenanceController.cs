using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MES.Contracts.Common;
using MES.Contracts.Phase1;
using MES.Services.Production;

namespace MES.Api.Controllers.Phase1;

/// <summary>
/// 设备故障/维护 API
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EquipmentMaintenanceController : ControllerBase
{
    private readonly IEquipmentMaintenanceService _equipmentMaintenanceService;
    private readonly ILogger<EquipmentMaintenanceController> _logger;

    public EquipmentMaintenanceController(IEquipmentMaintenanceService equipmentMaintenanceService, ILogger<EquipmentMaintenanceController> logger)
    {
        _equipmentMaintenanceService = equipmentMaintenanceService;
        _logger = logger;
    }

    /// <summary>
    /// 报告设备故障
    /// </summary>
    /// <param name="request">故障报告请求</param>
    /// <returns>设备故障响应</returns>
    [HttpPost("faults")]
    [ProducesResponseType(typeof(ApiResponse<EquipmentFaultResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<EquipmentFaultResponse>), 400)]
    public async Task<ApiResponse<EquipmentFaultResponse>> ReportFault([FromBody] ReportEquipmentFaultRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.EquipmentId))
                return ApiResponse<EquipmentFaultResponse>.Fail("设备ID不能为空");

            if (string.IsNullOrWhiteSpace(request.Description))
                return ApiResponse<EquipmentFaultResponse>.Fail("故障描述不能为空");

            var result = await _equipmentMaintenanceService.ReportFaultAsync(request);
            return ApiResponse<EquipmentFaultResponse>.Ok(result, "设备故障报告成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "报告设备故障时发生异常");
            return ApiResponse<EquipmentFaultResponse>.Fail("报告设备故障失败");
        }
    }

    /// <summary>
    /// 查询设备故障（分页）
    /// </summary>
    /// <param name="query">查询条件</param>
    /// <returns>分页故障列表</returns>
    [HttpGet("faults")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<EquipmentFaultResponse>>), 200)]
    public async Task<ApiResponse<PagedResult<EquipmentFaultResponse>>> GetFaults([FromQuery] EquipmentFaultQuery query)
    {
        try
        {
            var result = await _equipmentMaintenanceService.GetFaultsAsync(query);
            return ApiResponse<PagedResult<EquipmentFaultResponse>>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查询设备故障时发生异常");
            return ApiResponse<PagedResult<EquipmentFaultResponse>>.Fail("查询设备故障失败");
        }
    }

    /// <summary>
    /// 派修故障
    /// </summary>
    /// <param name="request">派修请求</param>
    /// <returns>是否成功</returns>
    [HttpPost("faults/dispatch")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponse<bool>), 400)]
    public async Task<ApiResponse<bool>> DispatchFault([FromBody] DispatchFaultRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.FaultId))
                return ApiResponse<bool>.Fail("故障ID不能为空");

            if (string.IsNullOrWhiteSpace(request.AssigneeId))
                return ApiResponse<bool>.Fail("指派人ID不能为空");

            var result = await _equipmentMaintenanceService.DispatchFaultAsync(request);
            return ApiResponse<bool>.Ok(result, "故障派修成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "派修故障时发生异常");
            return ApiResponse<bool>.Fail("故障派修失败");
        }
    }

    /// <summary>
    /// 完成维修
    /// </summary>
    /// <param name="faultId">故障ID</param>
    /// <param name="request">完成维修请求</param>
    /// <returns>是否成功</returns>
    [HttpPost("faults/{faultId}/complete")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponse<bool>), 400)]
    public async Task<ApiResponse<bool>> CompleteRepair(string faultId, [FromBody] CompleteRepairRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(faultId))
                return ApiResponse<bool>.Fail("故障ID不能为空");

            var result = await _equipmentMaintenanceService.CompleteRepairAsync(request);
            return ApiResponse<bool>.Ok(result, "维修完成");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "完成维修时发生异常");
            return ApiResponse<bool>.Fail("完成维修失败");
        }
    }

    /// <summary>
    /// 创建PM计划
    /// </summary>
    /// <param name="request">PM计划创建请求</param>
    /// <returns>PM计划响应</returns>
    [HttpPost("pm-plans")]
    [ProducesResponseType(typeof(ApiResponse<PmPlanResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<PmPlanResponse>), 400)]
    public async Task<ApiResponse<PmPlanResponse>> CreatePmPlan([FromBody] CreatePmPlanRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.EquipmentId))
                return ApiResponse<PmPlanResponse>.Fail("设备ID不能为空");

            var result = await _equipmentMaintenanceService.CreatePmPlanAsync(request);
            return ApiResponse<PmPlanResponse>.Ok(result, "PM计划创建成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建PM计划时发生异常");
            return ApiResponse<PmPlanResponse>.Fail("创建PM计划失败");
        }
    }

    /// <summary>
    /// 查询PM计划（分页）
    /// </summary>
    /// <param name="query">查询条件</param>
    /// <returns>分页PM计划列表</returns>
    [HttpGet("pm-plans")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<PmPlanResponse>>), 200)]
    public async Task<ApiResponse<PagedResult<PmPlanResponse>>> GetPmPlans([FromQuery] PmPlanQuery query)
    {
        try
        {
            var result = await _equipmentMaintenanceService.GetPmPlansAsync(query);
            return ApiResponse<PagedResult<PmPlanResponse>>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查询PM计划时发生异常");
            return ApiResponse<PagedResult<PmPlanResponse>>.Fail("查询PM计划失败");
        }
    }

    /// <summary>
    /// 执行PM
    /// </summary>
    /// <param name="planId">计划ID</param>
    /// <param name="request">PM执行请求</param>
    /// <returns>是否成功</returns>
    [HttpPost("pm-plans/{planId}/execute")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponse<bool>), 400)]
    public async Task<ApiResponse<bool>> ExecutePm(string planId, [FromBody] ExecutePmRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(planId))
                return ApiResponse<bool>.Fail("计划ID不能为空");

            var result = await _equipmentMaintenanceService.ExecutePmAsync(request);
            return ApiResponse<bool>.Ok(result, "PM执行成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "执行PM时发生异常");
            return ApiResponse<bool>.Fail("PM执行失败");
        }
    }

    /// <summary>
    /// 获取MTBF/MTTR统计
    /// </summary>
    /// <param name="equipmentId">设备ID</param>
    /// <param name="period">统计周期，默认6M</param>
    /// <returns>MTBF/MTTR统计列表</returns>
    [HttpGet("{equipmentId}/mtbf-mttr")]
    [ProducesResponseType(typeof(ApiResponse<List<MtbfMttrResponse>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<List<MtbfMttrResponse>>), 400)]
    public async Task<ApiResponse<List<MtbfMttrResponse>>> GetMtbfMttr(string equipmentId, [FromQuery] string period = "6M")
    {
        try
        {
            if (string.IsNullOrWhiteSpace(equipmentId))
                return ApiResponse<List<MtbfMttrResponse>>.Fail("设备ID不能为空");

            var result = await _equipmentMaintenanceService.GetMtbfMttrAsync(equipmentId, period);
            return ApiResponse<List<MtbfMttrResponse>>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取MTBF/MTTR统计时发生异常");
            return ApiResponse<List<MtbfMttrResponse>>.Fail("获取MTBF/MTTR统计失败");
        }
    }
}
