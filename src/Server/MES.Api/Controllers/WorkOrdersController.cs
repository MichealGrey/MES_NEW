using Microsoft.AspNetCore.Mvc;
using MES.Contracts.Production;
using MES.Contracts.Common;
using MES.Services.Production;

namespace MES.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkOrdersController : ControllerBase
{
    private readonly IWorkOrderService _workOrderService;

    public WorkOrdersController(IWorkOrderService workOrderService)
    {
        _workOrderService = workOrderService;
    }

    [HttpGet]
    public async Task<ApiResponse<PagedResult<WorkOrderDto>>> GetWorkOrders(
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null,
        [FromQuery] string? customerId = null,
        [FromQuery] string? keyword = null)
    {
        var result = await _workOrderService.GetPagedAsync(pageIndex, pageSize, status, customerId, keyword);
        return ApiResponse<PagedResult<WorkOrderDto>>.Ok(result);
    }

    [HttpGet("{orderId}")]
    public async Task<ApiResponse<WorkOrderDto?>> GetWorkOrder(string orderId)
    {
        var result = await _workOrderService.GetByIdAsync(orderId);
        return result != null
            ? ApiResponse<WorkOrderDto?>.Ok(result)
            : ApiResponse<WorkOrderDto?>.Fail("Work order not found");
    }

    [HttpPost]
    public async Task<ApiResponse<WorkOrderDto>> CreateWorkOrder([FromBody] CreateWorkOrderRequest request)
    {
        var result = await _workOrderService.CreateAsync(request);
        return ApiResponse<WorkOrderDto>.Ok(result, "Work order created");
    }

    [HttpPut("{orderId}")]
    public async Task<ApiResponse<bool>> UpdateWorkOrder(string orderId, [FromBody] UpdateWorkOrderRequest request)
    {
        request.OrderId = orderId;
        var result = await _workOrderService.UpdateAsync(request);
        return result
            ? ApiResponse<bool>.Ok(true, "Work order updated")
            : ApiResponse<bool>.Fail("Work order not found");
    }

    [HttpDelete("{orderId}")]
    public async Task<ApiResponse<bool>> DeleteWorkOrder(string orderId)
    {
        var result = await _workOrderService.DeleteAsync(orderId);
        return result
            ? ApiResponse<bool>.Ok(true, "Work order deleted")
            : ApiResponse<bool>.Fail("Work order not found");
    }

    [HttpGet("customer/{customerId}")]
    public async Task<ApiResponse<List<WorkOrderDto>>> GetByCustomer(string customerId)
    {
        var result = await _workOrderService.GetByCustomerIdAsync(customerId);
        return ApiResponse<List<WorkOrderDto>>.Ok(result);
    }
}
