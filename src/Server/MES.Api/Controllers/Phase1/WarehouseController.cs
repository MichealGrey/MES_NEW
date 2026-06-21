using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MES.Contracts.Common;
using MES.Contracts.Phase1;
using MES.Services.Warehouse;

namespace MES.Api.Controllers.Phase1;

/// <summary>
/// 仓库管理 API
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WarehouseController : ControllerBase
{
    private readonly IWarehouseService _warehouseService;
    private readonly ILogger<WarehouseController> _logger;

    public WarehouseController(IWarehouseService warehouseService, ILogger<WarehouseController> logger)
    {
        _warehouseService = warehouseService;
        _logger = logger;
    }

    /// <summary>
    /// 物料入库
    /// </summary>
    /// <param name="request">入库请求</param>
    /// <returns>入库响应</returns>
    [HttpPost("receipt")]
    [ProducesResponseType(typeof(ApiResponse<WarehouseReceiptResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<WarehouseReceiptResponse>), 400)]
    public async Task<ApiResponse<WarehouseReceiptResponse>> ReceiveMaterial([FromBody] WarehouseReceiptRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.BatchId))
                return ApiResponse<WarehouseReceiptResponse>.Fail("批次ID不能为空");

            if (string.IsNullOrWhiteSpace(request.MaterialId))
                return ApiResponse<WarehouseReceiptResponse>.Fail("物料ID不能为空");

            if (request.Quantity <= 0)
                return ApiResponse<WarehouseReceiptResponse>.Fail("数量必须大于0");

            var result = await _warehouseService.ReceiveMaterialAsync(request);
            return ApiResponse<WarehouseReceiptResponse>.Ok(result, "物料入库成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "物料入库时发生异常");
            return ApiResponse<WarehouseReceiptResponse>.Fail("物料入库失败");
        }
    }

    /// <summary>
    /// 查询库存（分页）
    /// </summary>
    /// <param name="query">查询条件</param>
    /// <returns>分页库存列表</returns>
    [HttpGet("inventory")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<InventoryResponse>>), 200)]
    public async Task<ApiResponse<PagedResult<InventoryResponse>>> GetInventory([FromQuery] InventoryQuery query)
    {
        try
        {
            var result = await _warehouseService.GetInventoryAsync(query);
            return ApiResponse<PagedResult<InventoryResponse>>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查询库存时发生异常");
            return ApiResponse<PagedResult<InventoryResponse>>.Fail("查询库存失败");
        }
    }

    /// <summary>
    /// 获取FIFO推荐
    /// </summary>
    /// <param name="request">FIFO推荐请求</param>
    /// <returns>FIFO推荐结果</returns>
    [HttpGet("fifo-recommend")]
    [ProducesResponseType(typeof(ApiResponse<FifoRecommendResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<FifoRecommendResponse>), 400)]
    public async Task<ApiResponse<FifoRecommendResponse>> GetFifoRecommendation([FromQuery] FifoRecommendRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.MaterialId))
                return ApiResponse<FifoRecommendResponse>.Fail("物料ID不能为空");

            if (request.RequestedQty <= 0)
                return ApiResponse<FifoRecommendResponse>.Fail("请求数量必须大于0");

            var result = await _warehouseService.GetFifoRecommendationAsync(request.MaterialId, request.RequestedQty);
            return ApiResponse<FifoRecommendResponse>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取FIFO推荐时发生异常");
            return ApiResponse<FifoRecommendResponse>.Fail("获取FIFO推荐失败");
        }
    }

    /// <summary>
    /// 获取有效期预警
    /// </summary>
    /// <param name="warningDays">预警天数，默认30天</param>
    /// <returns>有效期预警列表</returns>
    [HttpGet("expiry-warnings")]
    [ProducesResponseType(typeof(ApiResponse<List<ExpiryWarningResponse>>), 200)]
    public async Task<ApiResponse<List<ExpiryWarningResponse>>> GetExpiryWarnings([FromQuery] int warningDays = 30)
    {
        try
        {
            var result = await _warehouseService.GetExpiryWarningsAsync(warningDays);
            return ApiResponse<List<ExpiryWarningResponse>>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取有效期预警时发生异常");
            return ApiResponse<List<ExpiryWarningResponse>>.Fail("获取有效期预警失败");
        }
    }

    /// <summary>
    /// 锁定批次
    /// </summary>
    /// <param name="batchId">批次ID</param>
    /// <param name="request">批次锁定请求</param>
    /// <returns>是否成功</returns>
    [HttpPost("batches/{batchId}/lock")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponse<bool>), 400)]
    public async Task<ApiResponse<bool>> LockBatch(string batchId, [FromBody] BatchLockRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(batchId))
                return ApiResponse<bool>.Fail("批次ID不能为空");

            if (string.IsNullOrWhiteSpace(request.Reason))
                return ApiResponse<bool>.Fail("锁定原因不能为空");

            var result = await _warehouseService.LockBatchAsync(batchId, request.Reason, request.OperatorId);
            return ApiResponse<bool>.Ok(result, "批次锁定成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "锁定批次时发生异常");
            return ApiResponse<bool>.Fail("批次锁定失败");
        }
    }

    /// <summary>
    /// 解锁批次
    /// </summary>
    /// <param name="batchId">批次ID</param>
    /// <param name="request">批次解锁请求</param>
    /// <returns>是否成功</returns>
    [HttpPost("batches/{batchId}/unlock")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponse<bool>), 400)]
    public async Task<ApiResponse<bool>> UnlockBatch(string batchId, [FromBody] BatchUnlockRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(batchId))
                return ApiResponse<bool>.Fail("批次ID不能为空");

            var result = await _warehouseService.UnlockBatchAsync(batchId, request.OperatorId);
            return ApiResponse<bool>.Ok(result, "批次解锁成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "解锁批次时发生异常");
            return ApiResponse<bool>.Fail("批次解锁失败");
        }
    }

    /// <summary>
    /// 库存调整
    /// </summary>
    /// <param name="request">库存调整请求</param>
    /// <returns>是否成功</returns>
    [HttpPost("inventory/adjust")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponse<bool>), 400)]
    public async Task<ApiResponse<bool>> AdjustInventory([FromBody] InventoryAdjustRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.MaterialId))
                return ApiResponse<bool>.Fail("物料ID不能为空");

            if (string.IsNullOrWhiteSpace(request.BatchId))
                return ApiResponse<bool>.Fail("批次ID不能为空");

            if (request.AdjustQty == 0)
                return ApiResponse<bool>.Fail("调整数量不能为0");

            var result = await _warehouseService.AdjustInventoryAsync(request);
            return ApiResponse<bool>.Ok(result, "库存调整成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "库存调整时发生异常");
            return ApiResponse<bool>.Fail("库存调整失败");
        }
    }
}

/// <summary>
/// FIFO推荐请求
/// </summary>
public class FifoRecommendRequest
{
    /// <summary>
    /// 物料ID
    /// </summary>
    public string MaterialId { get; set; } = string.Empty;

    /// <summary>
    /// 请求数量
    /// </summary>
    public int RequestedQty { get; set; }
}

/// <summary>
/// 批次解锁请求
/// </summary>
public class BatchUnlockRequest
{
    /// <summary>
    /// 操作人工号
    /// </summary>
    public string OperatorId { get; set; } = string.Empty;
}
