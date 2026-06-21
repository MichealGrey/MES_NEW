using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MES.Contracts.Common;
using MES.Contracts.Phase1;
using MES.Services.Warehouse;

namespace MES.Api.Controllers.Phase1;

/// <summary>
/// 成品管理 API
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FinishedGoodsController : ControllerBase
{
    private readonly IFinishedGoodsService _finishedGoodsService;
    private readonly ILogger<FinishedGoodsController> _logger;

    public FinishedGoodsController(IFinishedGoodsService finishedGoodsService, ILogger<FinishedGoodsController> logger)
    {
        _finishedGoodsService = finishedGoodsService;
        _logger = logger;
    }

    /// <summary>
    /// 成品入库
    /// </summary>
    /// <param name="request">成品入库请求</param>
    /// <returns>入库响应</returns>
    [HttpPost("receipt")]
    [ProducesResponseType(typeof(ApiResponse<WarehouseReceiptResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<WarehouseReceiptResponse>), 400)]
    public async Task<ApiResponse<WarehouseReceiptResponse>> ReceiveFinishedGoods([FromBody] FinishedGoodsReceiptRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.WorkOrderId))
                return ApiResponse<WarehouseReceiptResponse>.Fail("工单ID不能为空");

            if (string.IsNullOrWhiteSpace(request.LotId))
                return ApiResponse<WarehouseReceiptResponse>.Fail("批次ID不能为空");

            if (request.Quantity <= 0)
                return ApiResponse<WarehouseReceiptResponse>.Fail("数量必须大于0");

            var result = await _finishedGoodsService.ReceiveFinishedGoodsAsync(request);
            return ApiResponse<WarehouseReceiptResponse>.Ok(result, "成品入库成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "成品入库时发生异常");
            return ApiResponse<WarehouseReceiptResponse>.Fail("成品入库失败");
        }
    }

    /// <summary>
    /// 查询成品库存（分页）
    /// </summary>
    /// <param name="query">查询条件</param>
    /// <returns>分页成品库存列表</returns>
    [HttpGet("inventory")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<FinishedGoodsInventoryResponse>>), 200)]
    public async Task<ApiResponse<PagedResult<FinishedGoodsInventoryResponse>>> GetInventory([FromQuery] FinishedGoodsQuery query)
    {
        try
        {
            var result = await _finishedGoodsService.GetInventoryAsync(query);
            return ApiResponse<PagedResult<FinishedGoodsInventoryResponse>>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查询成品库存时发生异常");
            return ApiResponse<PagedResult<FinishedGoodsInventoryResponse>>.Fail("查询成品库存失败");
        }
    }

    /// <summary>
    /// 成品发货
    /// </summary>
    /// <param name="request">成品发货请求</param>
    /// <returns>发货响应</returns>
    [HttpPost("ship")]
    [ProducesResponseType(typeof(ApiResponse<ShipmentResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<ShipmentResponse>), 400)]
    public async Task<ApiResponse<ShipmentResponse>> ShipFinishedGoods([FromBody] ShipFinishedGoodsRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.ShipmentId))
                return ApiResponse<ShipmentResponse>.Fail("出货单ID不能为空");

            if (request.Items == null || request.Items.Count == 0)
                return ApiResponse<ShipmentResponse>.Fail("发货项目不能为空");

            var result = await _finishedGoodsService.ShipFinishedGoodsAsync(request);
            return ApiResponse<ShipmentResponse>.Ok(result, "成品发货成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "成品发货时发生异常");
            return ApiResponse<ShipmentResponse>.Fail("成品发货失败");
        }
    }

    /// <summary>
    /// 查询发货记录（分页）
    /// </summary>
    /// <param name="query">查询条件</param>
    /// <returns>分页发货记录列表</returns>
    [HttpGet("shipments")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ShipmentResponse>>), 200)]
    public async Task<ApiResponse<PagedResult<ShipmentResponse>>> GetShipments([FromQuery] FinishedGoodsQuery query)
    {
        try
        {
            var result = await _finishedGoodsService.GetShipmentsAsync(query);
            return ApiResponse<PagedResult<ShipmentResponse>>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查询发货记录时发生异常");
            return ApiResponse<PagedResult<ShipmentResponse>>.Fail("查询发货记录失败");
        }
    }
}
