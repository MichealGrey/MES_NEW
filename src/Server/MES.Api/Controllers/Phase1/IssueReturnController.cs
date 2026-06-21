using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MES.Contracts.Common;
using MES.Contracts.Phase1;
using MES.Services.Warehouse;

namespace MES.Api.Controllers.Phase1;

/// <summary>
/// 物料领用/退料 API
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class IssueReturnController : ControllerBase
{
    private readonly IIssueReturnService _issueReturnService;
    private readonly ILogger<IssueReturnController> _logger;

    public IssueReturnController(IIssueReturnService issueReturnService, ILogger<IssueReturnController> logger)
    {
        _issueReturnService = issueReturnService;
        _logger = logger;
    }

    /// <summary>
    /// 领料
    /// </summary>
    /// <param name="request">领料请求</param>
    /// <returns>领料单响应</returns>
    [HttpPost("issue")]
    [ProducesResponseType(typeof(ApiResponse<IssueOrderResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<IssueOrderResponse>), 400)]
    public async Task<ApiResponse<IssueOrderResponse>> IssueMaterial([FromBody] IssueMaterialRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.WorkOrderId))
                return ApiResponse<IssueOrderResponse>.Fail("工单ID不能为空");

            if (request.Items == null || request.Items.Count == 0)
                return ApiResponse<IssueOrderResponse>.Fail("领料项目不能为空");

            var result = await _issueReturnService.IssueMaterialAsync(request);
            return ApiResponse<IssueOrderResponse>.Ok(result, "领料成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "领料时发生异常");
            return ApiResponse<IssueOrderResponse>.Fail("领料失败");
        }
    }

    /// <summary>
    /// 查询领料单（分页）
    /// </summary>
    /// <param name="query">查询条件</param>
    /// <returns>分页领料单列表</returns>
    [HttpGet("issue")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<IssueOrderResponse>>), 200)]
    public async Task<ApiResponse<PagedResult<IssueOrderResponse>>> GetIssueOrders([FromQuery] IssueOrderQuery query)
    {
        try
        {
            var result = await _issueReturnService.GetIssueOrdersAsync(query);
            return ApiResponse<PagedResult<IssueOrderResponse>>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查询领料单时发生异常");
            return ApiResponse<PagedResult<IssueOrderResponse>>.Fail("查询领料单失败");
        }
    }

    /// <summary>
    /// 齐套检查
    /// </summary>
    /// <param name="workOrderId">工单ID</param>
    /// <returns>齐套检查结果</returns>
    [HttpGet("kit-check/{workOrderId}")]
    [ProducesResponseType(typeof(ApiResponse<KitCheckResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<KitCheckResponse>), 400)]
    public async Task<ApiResponse<KitCheckResponse>> CheckKit(string workOrderId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(workOrderId))
                return ApiResponse<KitCheckResponse>.Fail("工单ID不能为空");

            var result = await _issueReturnService.CheckKitAsync(workOrderId);
            return ApiResponse<KitCheckResponse>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "齐套检查时发生异常");
            return ApiResponse<KitCheckResponse>.Fail("齐套检查失败");
        }
    }

    /// <summary>
    /// 跳过FIFO审批
    /// </summary>
    /// <param name="issueItemId">领料项目ID</param>
    /// <param name="request">跳过FIFO请求</param>
    /// <returns>是否成功</returns>
    [HttpPost("issue-items/{issueItemId}/skip-fifo")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponse<bool>), 400)]
    public async Task<ApiResponse<bool>> SkipFifoApproval(string issueItemId, [FromBody] SkipFifoRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(issueItemId))
                return ApiResponse<bool>.Fail("领料项目ID不能为空");

            if (string.IsNullOrWhiteSpace(request.Reason))
                return ApiResponse<bool>.Fail("跳过原因不能为空");

            var result = await _issueReturnService.SkipFifoApprovalAsync(issueItemId, request.ApprovedBy, request.Reason);
            return ApiResponse<bool>.Ok(result, "跳过FIFO审批成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "跳过FIFO审批时发生异常");
            return ApiResponse<bool>.Fail("跳过FIFO审批失败");
        }
    }

    /// <summary>
    /// 退料
    /// </summary>
    /// <param name="request">退料请求</param>
    /// <returns>退料单响应</returns>
    [HttpPost("return")]
    [ProducesResponseType(typeof(ApiResponse<ReturnOrderResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<ReturnOrderResponse>), 400)]
    public async Task<ApiResponse<ReturnOrderResponse>> ReturnMaterial([FromBody] ReturnMaterialRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.WorkOrderId))
                return ApiResponse<ReturnOrderResponse>.Fail("工单ID不能为空");

            if (request.Items == null || request.Items.Count == 0)
                return ApiResponse<ReturnOrderResponse>.Fail("退料项目不能为空");

            var result = await _issueReturnService.ReturnMaterialAsync(request);
            return ApiResponse<ReturnOrderResponse>.Ok(result, "退料成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "退料时发生异常");
            return ApiResponse<ReturnOrderResponse>.Fail("退料失败");
        }
    }

    /// <summary>
    /// 查询退料单（分页）
    /// </summary>
    /// <param name="query">查询条件</param>
    /// <returns>分页退料单列表</returns>
    [HttpGet("return")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ReturnOrderResponse>>), 200)]
    public async Task<ApiResponse<PagedResult<ReturnOrderResponse>>> GetReturnOrders([FromQuery] ReturnOrderQuery query)
    {
        try
        {
            var result = await _issueReturnService.GetReturnOrdersAsync(query);
            return ApiResponse<PagedResult<ReturnOrderResponse>>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查询退料单时发生异常");
            return ApiResponse<PagedResult<ReturnOrderResponse>>.Fail("查询退料单失败");
        }
    }
}

/// <summary>
/// 跳过FIFO审批请求
/// </summary>
public class SkipFifoRequest
{
    /// <summary>
    /// 审批人工号
    /// </summary>
    public string ApprovedBy { get; set; } = string.Empty;

    /// <summary>
    /// 跳过原因
    /// </summary>
    public string Reason { get; set; } = string.Empty;
}
