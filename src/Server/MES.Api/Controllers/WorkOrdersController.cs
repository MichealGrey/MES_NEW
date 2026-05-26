using Microsoft.AspNetCore.Mvc;
using MES.Contracts.Production;
using MES.Contracts.Common;

namespace MES.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkOrdersController : ControllerBase
{
    [HttpGet]
    public ApiResponse<PagedResult<WorkOrderDto>> GetWorkOrders()
    {
        return ApiResponse<PagedResult<WorkOrderDto>>.Ok(new PagedResult<WorkOrderDto>
        {
            Items = [],
            TotalCount = 0,
            PageIndex = 1,
            PageSize = 20
        });
    }
}
