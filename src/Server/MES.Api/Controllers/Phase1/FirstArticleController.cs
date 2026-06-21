using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MES.Contracts.Common;
using MES.Contracts.Phase1;
using MES.Services.Quality;

namespace MES.Api.Controllers.Phase1;

/// <summary>
/// 首件检验 API
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FirstArticleController : ControllerBase
{
    private readonly IFirstArticleService _firstArticleService;
    private readonly ILogger<FirstArticleController> _logger;

    public FirstArticleController(IFirstArticleService firstArticleService, ILogger<FirstArticleController> logger)
    {
        _firstArticleService = firstArticleService;
        _logger = logger;
    }

    /// <summary>
    /// 创建首件检验
    /// </summary>
    /// <param name="request">首件检验创建请求</param>
    /// <returns>首件检验响应</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<FirstArticleResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<FirstArticleResponse>), 400)]
    public async Task<ApiResponse<FirstArticleResponse>> Create([FromBody] CreateFirstArticleRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.WorkOrderId))
                return ApiResponse<FirstArticleResponse>.Fail("工单ID不能为空");

            if (string.IsNullOrWhiteSpace(request.LotId))
                return ApiResponse<FirstArticleResponse>.Fail("批次ID不能为空");

            if (string.IsNullOrWhiteSpace(request.TriggerReason))
                return ApiResponse<FirstArticleResponse>.Fail("触发原因不能为空");

            var result = await _firstArticleService.CreateAsync(request);
            return ApiResponse<FirstArticleResponse>.Ok(result, "首件检验创建成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建首件检验时发生异常");
            return ApiResponse<FirstArticleResponse>.Fail("创建首件检验失败");
        }
    }

    /// <summary>
    /// 查询首件检验（分页）
    /// </summary>
    /// <param name="query">查询条件</param>
    /// <returns>分页首件检验列表</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<FirstArticleResponse>>), 200)]
    public async Task<ApiResponse<PagedResult<FirstArticleResponse>>> GetFirstArticles([FromQuery] FirstArticleQuery query)
    {
        try
        {
            var result = await _firstArticleService.GetFirstArticlesAsync(query);
            return ApiResponse<PagedResult<FirstArticleResponse>>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查询首件检验时发生异常");
            return ApiResponse<PagedResult<FirstArticleResponse>>.Fail("查询首件检验失败");
        }
    }

    /// <summary>
    /// 获取首件检验详情
    /// </summary>
    /// <param name="faId">首件ID</param>
    /// <returns>首件检验详情</returns>
    [HttpGet("{faId}")]
    [ProducesResponseType(typeof(ApiResponse<FirstArticleResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<FirstArticleResponse>), 404)]
    public async Task<ApiResponse<FirstArticleResponse>> GetDetail(string faId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(faId))
                return ApiResponse<FirstArticleResponse>.Fail("首件ID不能为空");

            var result = await _firstArticleService.GetDetailAsync(faId);
            if (result == null)
                return ApiResponse<FirstArticleResponse>.Fail("首件检验不存在");

            return ApiResponse<FirstArticleResponse>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取首件检验详情时发生异常");
            return ApiResponse<FirstArticleResponse>.Fail("获取首件检验详情失败");
        }
    }

    /// <summary>
    /// 执行首件检验
    /// </summary>
    /// <param name="faId">首件ID</param>
    /// <param name="request">执行首件检验请求</param>
    /// <returns>首件检验响应</returns>
    [HttpPost("{faId}/execute")]
    [ProducesResponseType(typeof(ApiResponse<FirstArticleResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<FirstArticleResponse>), 400)]
    public async Task<ApiResponse<FirstArticleResponse>> Execute(string faId, [FromBody] ExecuteFirstArticleRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(faId))
                return ApiResponse<FirstArticleResponse>.Fail("首件ID不能为空");

            var result = await _firstArticleService.ExecuteAsync(request);
            return ApiResponse<FirstArticleResponse>.Ok(result, "首件检验执行成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "执行首件检验时发生异常");
            return ApiResponse<FirstArticleResponse>.Fail("执行首件检验失败");
        }
    }

    /// <summary>
    /// 确认首件检验
    /// </summary>
    /// <param name="faId">首件ID</param>
    /// <param name="request">确认首件检验请求</param>
    /// <returns>首件检验响应</returns>
    [HttpPost("{faId}/confirm")]
    [ProducesResponseType(typeof(ApiResponse<FirstArticleResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<FirstArticleResponse>), 400)]
    public async Task<ApiResponse<FirstArticleResponse>> Confirm(string faId, [FromBody] ConfirmFirstArticleRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(faId))
                return ApiResponse<FirstArticleResponse>.Fail("首件ID不能为空");

            if (string.IsNullOrWhiteSpace(request.Confirmation))
                return ApiResponse<FirstArticleResponse>.Fail("确认结果不能为空");

            var result = await _firstArticleService.ConfirmAsync(request);
            return ApiResponse<FirstArticleResponse>.Ok(result, "首件检验确认成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "确认首件检验时发生异常");
            return ApiResponse<FirstArticleResponse>.Fail("确认首件检验失败");
        }
    }

    /// <summary>
    /// 驳回首件检验
    /// </summary>
    /// <param name="faId">首件ID</param>
    /// <param name="request">驳回首件检验请求</param>
    /// <returns>是否成功</returns>
    [HttpPost("{faId}/reject")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponse<bool>), 400)]
    public async Task<ApiResponse<bool>> Reject(string faId, [FromBody] RejectFirstArticleRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(faId))
                return ApiResponse<bool>.Fail("首件ID不能为空");

            if (string.IsNullOrWhiteSpace(request.RejectionReason))
                return ApiResponse<bool>.Fail("驳回原因不能为空");

            var result = await _firstArticleService.RejectAsync(request);
            return ApiResponse<bool>.Ok(result, "首件检验驳回成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "驳回首件检验时发生异常");
            return ApiResponse<bool>.Fail("首件检验驳回失败");
        }
    }

    /// <summary>
    /// 记录拉力测试
    /// </summary>
    /// <param name="request">拉力测试记录请求</param>
    /// <returns>拉力测试响应</returns>
    [HttpPost("bond-pull-test")]
    [ProducesResponseType(typeof(ApiResponse<BondPullTestResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<BondPullTestResponse>), 400)]
    public async Task<ApiResponse<BondPullTestResponse>> RecordBondPullTest([FromBody] BondPullTestRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.LotId))
                return ApiResponse<BondPullTestResponse>.Fail("批次ID不能为空");

            if (request.PullForceGrams <= 0)
                return ApiResponse<BondPullTestResponse>.Fail("拉力值必须大于0");

            var result = await _firstArticleService.RecordBondPullTestAsync(request);
            return ApiResponse<BondPullTestResponse>.Ok(result, "拉力测试记录成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "记录拉力测试时发生异常");
            return ApiResponse<BondPullTestResponse>.Fail("记录拉力测试失败");
        }
    }

    /// <summary>
    /// 查询拉力测试记录
    /// </summary>
    /// <param name="lotId">批次ID</param>
    /// <param name="workOrderId">工单ID（可选）</param>
    /// <returns>拉力测试记录列表</returns>
    [HttpGet("bond-pull-test")]
    [ProducesResponseType(typeof(ApiResponse<List<BondPullTestResponse>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<List<BondPullTestResponse>>), 400)]
    public async Task<ApiResponse<List<BondPullTestResponse>>> GetBondPullTests([FromQuery] string lotId, [FromQuery] string? workOrderId = null)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(lotId))
                return ApiResponse<List<BondPullTestResponse>>.Fail("批次ID不能为空");

            var result = await _firstArticleService.GetBondPullTestsAsync(lotId, workOrderId);
            return ApiResponse<List<BondPullTestResponse>>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查询拉力测试记录时发生异常");
            return ApiResponse<List<BondPullTestResponse>>.Fail("查询拉力测试记录失败");
        }
    }
}
