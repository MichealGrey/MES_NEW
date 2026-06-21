using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MES.Contracts.Common;
using MES.Contracts.Phase1;
using MES.Services.Quality;

namespace MES.Api.Controllers.Phase1;

/// <summary>
/// 不合格品/MRB API
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NonconformingController : ControllerBase
{
    private readonly INonconformingService _nonconformingService;
    private readonly ILogger<NonconformingController> _logger;

    public NonconformingController(INonconformingService nonconformingService, ILogger<NonconformingController> logger)
    {
        _nonconformingService = nonconformingService;
        _logger = logger;
    }

    /// <summary>
    /// 创建不合格品记录
    /// </summary>
    /// <param name="request">不合格品创建请求</param>
    /// <returns>不合格品记录响应</returns>
    [HttpPost("records")]
    [ProducesResponseType(typeof(ApiResponse<NonconformingRecordResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<NonconformingRecordResponse>), 400)]
    public async Task<ApiResponse<NonconformingRecordResponse>> CreateRecord([FromBody] CreateNonconformingRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.LotId))
                return ApiResponse<NonconformingRecordResponse>.Fail("批次ID不能为空");

            if (string.IsNullOrWhiteSpace(request.DefectCode))
                return ApiResponse<NonconformingRecordResponse>.Fail("缺陷代码不能为空");

            if (request.AffectedQty <= 0)
                return ApiResponse<NonconformingRecordResponse>.Fail("受影响数量必须大于0");

            var result = await _nonconformingService.CreateRecordAsync(request);
            return ApiResponse<NonconformingRecordResponse>.Ok(result, "不合格品记录创建成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建不合格品记录时发生异常");
            return ApiResponse<NonconformingRecordResponse>.Fail("创建不合格品记录失败");
        }
    }

    /// <summary>
    /// 查询不合格品记录列表（分页）
    /// </summary>
    /// <param name="query">查询条件</param>
    /// <returns>分页记录列表</returns>
    [HttpGet("records")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<NonconformingRecordResponse>>), 200)]
    public async Task<ApiResponse<PagedResult<NonconformingRecordResponse>>> GetRecords([FromQuery] NonconformingQuery query)
    {
        try
        {
            var result = await _nonconformingService.GetRecordsAsync(query);
            return ApiResponse<PagedResult<NonconformingRecordResponse>>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查询不合格品记录时发生异常");
            return ApiResponse<PagedResult<NonconformingRecordResponse>>.Fail("查询不合格品记录失败");
        }
    }

    /// <summary>
    /// 获取不合格品详情
    /// </summary>
    /// <param name="ncrId">记录ID</param>
    /// <returns>记录详情</returns>
    [HttpGet("records/{ncrId}")]
    [ProducesResponseType(typeof(ApiResponse<NonconformingRecordResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<NonconformingRecordResponse>), 404)]
    public async Task<ApiResponse<NonconformingRecordResponse>> GetDetail(string ncrId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(ncrId))
                return ApiResponse<NonconformingRecordResponse>.Fail("记录ID不能为空");

            var result = await _nonconformingService.GetDetailAsync(ncrId);
            if (result == null)
                return ApiResponse<NonconformingRecordResponse>.Fail("记录不存在");

            return ApiResponse<NonconformingRecordResponse>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取不合格品详情时发生异常");
            return ApiResponse<NonconformingRecordResponse>.Fail("获取详情失败");
        }
    }

    /// <summary>
    /// 隔离不合格品
    /// </summary>
    /// <param name="ncrId">记录ID</param>
    /// <param name="request">操作人请求</param>
    /// <returns>是否成功</returns>
    [HttpPost("records/{ncrId}/isolate")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponse<bool>), 400)]
    public async Task<ApiResponse<bool>> Isolate(string ncrId, [FromBody] OperatorRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(ncrId))
                return ApiResponse<bool>.Fail("记录ID不能为空");

            var result = await _nonconformingService.IsolateAsync(ncrId, request.OperatorId);
            return ApiResponse<bool>.Ok(result, "不合格品隔离成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "隔离不合格品时发生异常");
            return ApiResponse<bool>.Fail("不合格品隔离失败");
        }
    }

    /// <summary>
    /// 创建MRB评审
    /// </summary>
    /// <param name="ncrId">记录ID</param>
    /// <param name="request">创建人请求</param>
    /// <returns>MRB评审响应</returns>
    [HttpPost("records/{ncrId}/mrb")]
    [ProducesResponseType(typeof(ApiResponse<MrbReviewResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<MrbReviewResponse>), 400)]
    public async Task<ApiResponse<MrbReviewResponse>> CreateMrb(string ncrId, [FromBody] CreatedByRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(ncrId))
                return ApiResponse<MrbReviewResponse>.Fail("记录ID不能为空");

            var result = await _nonconformingService.CreateMrbAsync(ncrId, request.CreatedBy);
            return ApiResponse<MrbReviewResponse>.Ok(result, "MRB评审创建成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建MRB评审时发生异常");
            return ApiResponse<MrbReviewResponse>.Fail("创建MRB评审失败");
        }
    }

    /// <summary>
    /// MRB投票
    /// </summary>
    /// <param name="mrbId">MRB评审ID</param>
    /// <param name="request">投票请求</param>
    /// <returns>MRB评审响应</returns>
    [HttpPost("mrb/{mrbId}/vote")]
    [ProducesResponseType(typeof(ApiResponse<MrbReviewResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<MrbReviewResponse>), 400)]
    public async Task<ApiResponse<MrbReviewResponse>> VoteMrb(string mrbId, [FromBody] MrbVoteRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(mrbId))
                return ApiResponse<MrbReviewResponse>.Fail("MRB评审ID不能为空");

            if (string.IsNullOrWhiteSpace(request.Vote))
                return ApiResponse<MrbReviewResponse>.Fail("投票不能为空");

            var result = await _nonconformingService.VoteMrbAsync(mrbId, request);
            return ApiResponse<MrbReviewResponse>.Ok(result, "MRB投票成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "MRB投票时发生异常");
            return ApiResponse<MrbReviewResponse>.Fail("MRB投票失败");
        }
    }

    /// <summary>
    /// 执行处置
    /// </summary>
    /// <param name="ncrId">记录ID</param>
    /// <param name="request">处置请求</param>
    /// <returns>是否成功</returns>
    [HttpPost("records/{ncrId}/disposition")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponse<bool>), 400)]
    public async Task<ApiResponse<bool>> ExecuteDisposition(string ncrId, [FromBody] DispositionRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(ncrId))
                return ApiResponse<bool>.Fail("记录ID不能为空");

            if (string.IsNullOrWhiteSpace(request.Disposition))
                return ApiResponse<bool>.Fail("处置方式不能为空");

            var result = await _nonconformingService.ExecuteDispositionAsync(ncrId, request);
            return ApiResponse<bool>.Ok(result, "处置执行成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "执行处置时发生异常");
            return ApiResponse<bool>.Fail("处置执行失败");
        }
    }

    /// <summary>
    /// 返工验证
    /// </summary>
    /// <param name="ncrId">记录ID</param>
    /// <param name="request">返工验证请求</param>
    /// <returns>是否成功</returns>
    [HttpPost("records/{ncrId}/rework-verify")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponse<bool>), 400)]
    public async Task<ApiResponse<bool>> ReworkVerify(string ncrId, [FromBody] ReworkVerifyRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(ncrId))
                return ApiResponse<bool>.Fail("记录ID不能为空");

            if (string.IsNullOrWhiteSpace(request.Result))
                return ApiResponse<bool>.Fail("验证结果不能为空");

            var result = await _nonconformingService.ReworkVerifyAsync(ncrId, request.Result, request.VerifiedBy);
            return ApiResponse<bool>.Ok(result, "返工验证成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "返工验证时发生异常");
            return ApiResponse<bool>.Fail("返工验证失败");
        }
    }
}

/// <summary>
/// 操作人请求
/// </summary>
public class OperatorRequest
{
    /// <summary>
    /// 操作人工号
    /// </summary>
    public string OperatorId { get; set; } = string.Empty;
}

/// <summary>
/// 创建人请求
/// </summary>
public class CreatedByRequest
{
    /// <summary>
    /// 创建人工号
    /// </summary>
    public string CreatedBy { get; set; } = string.Empty;
}

/// <summary>
/// 返工验证请求
/// </summary>
public class ReworkVerifyRequest
{
    /// <summary>
    /// 验证结果
    /// </summary>
    public string Result { get; set; } = string.Empty;

    /// <summary>
    /// 验证人工号
    /// </summary>
    public string VerifiedBy { get; set; } = string.Empty;
}
