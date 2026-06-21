using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MES.Contracts.Common;
using MES.Adapters.Abstractions;
using MES.Adapters.Abstractions.Erp;
using MES.Adapters.Abstractions.Eap;
using MES.Adapters.Abstractions.Wms;
using MES.Adapters.Abstractions.Qms;
using MES.Adapters.Abstractions.Oa;
using MES.Adapters.Abstractions.CustomerPortal;
using ErpDtos = MES.Adapters.Abstractions.Erp;
using EapDtos = MES.Adapters.Abstractions.Eap;
using WmsDtos = MES.Adapters.Abstractions.Wms;
using QmsDtos = MES.Adapters.Abstractions.Qms;
using OaDtos = MES.Adapters.Abstractions.Oa;
using PortalDtos = MES.Adapters.Abstractions.CustomerPortal;

namespace MES.Api.Controllers.Phase4;

/// <summary>
/// 外部系统集成适配器管理接口
/// </summary>
[ApiController]
[Route("api/v4/[controller]")]
[Authorize]
public class IntegrationController : ControllerBase
{
    private readonly IMesErpAdapter _erpAdapter;
    private readonly IMesEapAdapter _eapAdapter;
    private readonly IMesWmsAdapter _wmsAdapter;
    private readonly IMesQmsAdapter _qmsAdapter;
    private readonly IMesOaAdapter _oaAdapter;
    private readonly IMesCustomerPortalAdapter _portalAdapter;
    private readonly ILogger<IntegrationController> _logger;

    public IntegrationController(
        IMesErpAdapter erpAdapter,
        IMesEapAdapter eapAdapter,
        IMesWmsAdapter wmsAdapter,
        IMesQmsAdapter qmsAdapter,
        IMesOaAdapter oaAdapter,
        IMesCustomerPortalAdapter portalAdapter,
        ILogger<IntegrationController> logger)
    {
        _erpAdapter = erpAdapter;
        _eapAdapter = eapAdapter;
        _wmsAdapter = wmsAdapter;
        _qmsAdapter = qmsAdapter;
        _oaAdapter = oaAdapter;
        _portalAdapter = portalAdapter;
        _logger = logger;
    }

    /// <summary>
    /// 获取所有适配器信息和健康状态
    /// </summary>
    [HttpGet("adapters")]
    public async Task<IActionResult> GetAllAdapters()
    {
        try
        {
            var adapters = new List<AdapterStatusDto>();

            await AddAdapterStatusAsync(adapters, "ERP", _erpAdapter);
            await AddAdapterStatusAsync(adapters, "EAP", _eapAdapter);
            await AddAdapterStatusAsync(adapters, "WMS", _wmsAdapter);
            await AddAdapterStatusAsync(adapters, "QMS", _qmsAdapter);
            await AddAdapterStatusAsync(adapters, "OA", _oaAdapter);
            await AddAdapterStatusAsync(adapters, "CustomerPortal", _portalAdapter);

            return Ok(ApiResponse<List<AdapterStatusDto>>.Ok(adapters, "Retrieved all adapter statuses"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get adapter statuses");
            return BadRequest(ApiResponse<object>.Fail("Failed to retrieve adapter statuses"));
        }
    }

    /// <summary>
    /// 检查指定适配器健康状态
    /// </summary>
    [HttpGet("adapters/{adapterName}/health")]
    public async Task<IActionResult> CheckAdapterHealth(string adapterName)
    {
        try
        {
            var adapter = GetAdapterByName(adapterName);
            var result = await adapter.HealthCheckAsync();
            return Ok(ApiResponse<AdapterResult<HealthStatus>>.Ok(result, "Health check completed"));
        }
        catch (KeyNotFoundException)
        {
            return NotFound(ApiResponse<object>.Fail($"Adapter '{adapterName}' not found"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check health for adapter {AdapterName}", adapterName);
            return BadRequest(ApiResponse<object>.Fail($"Health check failed for {adapterName}"));
        }
    }

    /// <summary>
    /// 获取指定适配器信息
    /// </summary>
    [HttpGet("adapters/{adapterName}/info")]
    public IActionResult GetAdapterInfo(string adapterName)
    {
        try
        {
            var adapter = GetAdapterByName(adapterName);
            var info = adapter.GetAdapterInfo();
            return Ok(ApiResponse<AdapterInfo>.Ok(info, "Adapter info retrieved"));
        }
        catch (KeyNotFoundException)
        {
            return NotFound(ApiResponse<object>.Fail($"Adapter '{adapterName}' not found"));
        }
    }

    // ==================== ERP 集成接口 ====================

    /// <summary>
    /// 从ERP同步销售订单
    /// </summary>
    [HttpPost("erp/sync-orders")]
    public async Task<IActionResult> SyncErpOrders([FromBody] ErpDtos.OrderSyncRequest request)
    {
        try
        {
            var result = await _erpAdapter.SyncSalesOrdersAsync(request);
            _logger.LogInformation("ERP order sync completed: {Success}, Synced={Count}",
                result.Success, result.Data?.SyncedCount);
            return Ok(ApiResponse<object>.Ok(result, "ERP order sync completed"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to sync ERP orders");
            return BadRequest(ApiResponse<object>.Fail("ERP order sync failed"));
        }
    }

    /// <summary>
    /// 从ERP同步BOM
    /// </summary>
    [HttpPost("erp/sync-bom")]
    public async Task<IActionResult> SyncErpBom([FromBody] ErpDtos.BomSyncRequest request)
    {
        try
        {
            var result = await _erpAdapter.SyncBomAsync(request);
            return Ok(ApiResponse<object>.Ok(result, "ERP BOM sync completed"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to sync ERP BOM");
            return BadRequest(ApiResponse<object>.Fail("ERP BOM sync failed"));
        }
    }

    /// <summary>
    /// 从ERP同步物料主数据
    /// </summary>
    [HttpPost("erp/sync-materials")]
    public async Task<IActionResult> SyncErpMaterials([FromBody] ErpDtos.MaterialSyncRequest request)
    {
        try
        {
            var result = await _erpAdapter.SyncMaterialsAsync(request);
            return Ok(ApiResponse<object>.Ok(result, "ERP material sync completed"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to sync ERP materials");
            return BadRequest(ApiResponse<object>.Fail("ERP material sync failed"));
        }
    }

    /// <summary>
    /// 向ERP回传工单完工数据
    /// </summary>
    [HttpPost("erp/report-completion")]
    public async Task<IActionResult> ReportCompletion([FromBody] ErpDtos.CompletionData request)
    {
        try
        {
            var result = await _erpAdapter.ReportWorkOrderCompletionAsync(request);
            return Ok(ApiResponse<object>.Ok(result, "Work order completion reported to ERP"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to report completion to ERP");
            return BadRequest(ApiResponse<object>.Fail("Completion report failed"));
        }
    }

    /// <summary>
    /// 向ERP回传物料消耗数据
    /// </summary>
    [HttpPost("erp/report-consume")]
    public async Task<IActionResult> ReportConsume([FromBody] ErpDtos.ConsumeData request)
    {
        try
        {
            var result = await _erpAdapter.ReportMaterialConsumeAsync(request);
            return Ok(ApiResponse<object>.Ok(result, "Material consumption reported to ERP"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to report consumption to ERP");
            return BadRequest(ApiResponse<object>.Fail("Consumption report failed"));
        }
    }

    /// <summary>
    /// 向ERP回传成品入库数据
    /// </summary>
    [HttpPost("erp/report-receipt")]
    public async Task<IActionResult> ReportReceipt([FromBody] ErpDtos.ReceiptData request)
    {
        try
        {
            var result = await _erpAdapter.ReportFinishedGoodsReceiptAsync(request);
            return Ok(ApiResponse<object>.Ok(result, "Finished goods receipt reported to ERP"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to report receipt to ERP");
            return BadRequest(ApiResponse<object>.Fail("Receipt report failed"));
        }
    }

    // ==================== EAP 集成接口 ====================

    /// <summary>
    /// 下发工艺参数到设备
    /// </summary>
    [HttpPost("eap/download-params")]
    public async Task<IActionResult> DownloadParams([FromBody] EapDtos.ParameterSetData request)
    {
        try
        {
            var result = await _eapAdapter.DownloadParametersAsync(request);
            return Ok(ApiResponse<object>.Ok(result, "Parameters downloaded to equipment"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to download parameters");
            return BadRequest(ApiResponse<object>.Fail("Parameter download failed"));
        }
    }

    /// <summary>
    /// 获取设备状态
    /// </summary>
    [HttpGet("eap/equipment-status/{equipmentId}")]
    public async Task<IActionResult> GetEquipmentStatus(string equipmentId)
    {
        try
        {
            var result = await _eapAdapter.GetEquipmentStatusAsync(equipmentId);
            return Ok(ApiResponse<object>.Ok(result, "Equipment status retrieved"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get equipment status for {EquipmentId}", equipmentId);
            return BadRequest(ApiResponse<object>.Fail("Failed to get equipment status"));
        }
    }

    /// <summary>
    /// 采集设备生产数据
    /// </summary>
    [HttpPost("eap/collect-data/{equipmentId}")]
    public async Task<IActionResult> CollectData(string equipmentId)
    {
        try
        {
            var result = await _eapAdapter.CollectProcessDataAsync(equipmentId);
            return Ok(ApiResponse<object>.Ok(result, "Process data collected"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to collect process data for {EquipmentId}", equipmentId);
            return BadRequest(ApiResponse<object>.Fail("Data collection failed"));
        }
    }

    // ==================== WMS 集成接口 ====================

    /// <summary>
    /// 向WMS发送物料需求
    /// </summary>
    [HttpPost("wms/material-request")]
    public async Task<IActionResult> SendMaterialRequest([FromBody] WmsDtos.MaterialRequest request)
    {
        try
        {
            var result = await _wmsAdapter.SendMaterialRequestAsync(request);
            return Ok(ApiResponse<object>.Ok(result, "Material request sent to WMS"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send material request to WMS");
            return BadRequest(ApiResponse<object>.Fail("Material request failed"));
        }
    }

    /// <summary>
    /// 从WMS获取库存信息
    /// </summary>
    [HttpPost("wms/inventory")]
    public async Task<IActionResult> GetInventory([FromBody] WmsDtos.InventoryQuery query)
    {
        try
        {
            var result = await _wmsAdapter.GetInventoryAsync(query);
            return Ok(ApiResponse<object>.Ok(result, "Inventory retrieved from WMS"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get inventory from WMS");
            return BadRequest(ApiResponse<object>.Fail("Inventory query failed"));
        }
    }

    // ==================== QMS 集成接口 ====================

    /// <summary>
    /// 向QMS推送检验数据
    /// </summary>
    [HttpPost("qms/push-inspection")]
    public async Task<IActionResult> PushInspection([FromBody] QmsDtos.InspectionData request)
    {
        try
        {
            var result = await _qmsAdapter.PushInspectionDataAsync(request);
            return Ok(ApiResponse<object>.Ok(result, "Inspection data pushed to QMS"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to push inspection data to QMS");
            return BadRequest(ApiResponse<object>.Fail("Inspection push failed"));
        }
    }

    /// <summary>
    /// 从QMS拉取检验标准
    /// </summary>
    [HttpPost("qms/pull-standard")]
    public async Task<IActionResult> PullStandard([FromBody] QmsDtos.StandardQuery query)
    {
        try
        {
            var result = await _qmsAdapter.PullInspectionStandardAsync(query);
            return Ok(ApiResponse<object>.Ok(result, "Inspection standard retrieved from QMS"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to pull inspection standard from QMS");
            return BadRequest(ApiResponse<object>.Fail("Standard pull failed"));
        }
    }

    /// <summary>
    /// 接收QMS质量警报
    /// </summary>
    [HttpGet("qms/alert")]
    public async Task<IActionResult> GetQualityAlert()
    {
        try
        {
            var result = await _qmsAdapter.ReceiveQualityAlertAsync();
            return Ok(ApiResponse<object>.Ok(result, "Quality alert retrieved"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to receive quality alert");
            return BadRequest(ApiResponse<object>.Fail("Alert retrieval failed"));
        }
    }

    // ==================== OA 集成接口 ====================

    /// <summary>
    /// 向OA推送审批请求
    /// </summary>
    [HttpPost("oa/push-approval")]
    public async Task<IActionResult> PushApproval([FromBody] OaDtos.ApprovalRequest request)
    {
        try
        {
            var result = await _oaAdapter.PushApprovalRequestAsync(request);
            return Ok(ApiResponse<object>.Ok(result, "Approval request pushed to OA"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to push approval request to OA");
            return BadRequest(ApiResponse<object>.Fail("Approval push failed"));
        }
    }

    /// <summary>
    /// 查询OA审批状态
    /// </summary>
    [HttpGet("oa/approval-status/{approvalId}")]
    public async Task<IActionResult> GetApprovalStatus(string approvalId)
    {
        try
        {
            var result = await _oaAdapter.GetApprovalStatusAsync(approvalId);
            return Ok(ApiResponse<object>.Ok(result, "Approval status retrieved"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get approval status for {ApprovalId}", approvalId);
            return BadRequest(ApiResponse<object>.Fail("Status query failed"));
        }
    }

    // ==================== 客户门户集成接口 ====================

    /// <summary>
    /// 接收客户门户订单
    /// </summary>
    [HttpPost("portal/order")]
    public async Task<IActionResult> ReceivePortalOrder([FromBody] PortalDtos.PortalOrder request)
    {
        try
        {
            var result = await _portalAdapter.ReceivePortalOrderAsync(request);
            return Ok(ApiResponse<object>.Ok(result, "Portal order received"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to receive portal order");
            return BadRequest(ApiResponse<object>.Fail("Portal order reception failed"));
        }
    }

    /// <summary>
    /// 查询订单进度（客户门户）
    /// </summary>
    [HttpGet("portal/order-progress/{orderId}")]
    public async Task<IActionResult> GetOrderProgress(string orderId)
    {
        try
        {
            var result = await _portalAdapter.GetOrderProgressAsync(orderId);
            return Ok(ApiResponse<object>.Ok(result, "Order progress retrieved"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get order progress for {OrderId}", orderId);
            return BadRequest(ApiResponse<object>.Fail("Progress query failed"));
        }
    }

    /// <summary>
    /// 获取产品质量报告
    /// </summary>
    [HttpPost("portal/quality-report")]
    public async Task<IActionResult> GetQualityReport([FromBody] PortalDtos.ReportQuery query)
    {
        try
        {
            var result = await _portalAdapter.GetQualityReportAsync(query);
            return Ok(ApiResponse<object>.Ok(result, "Quality report retrieved"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get quality report");
            return BadRequest(ApiResponse<object>.Fail("Report retrieval failed"));
        }
    }

    // ==================== 私有方法 ====================

    private IMesAdapter GetAdapterByName(string adapterName) => adapterName.ToLowerInvariant() switch
    {
        "erp" => _erpAdapter,
        "eap" => _eapAdapter,
        "wms" => _wmsAdapter,
        "qms" => _qmsAdapter,
        "oa" => _oaAdapter,
        "customerportal" or "portal" => _portalAdapter,
        _ => throw new KeyNotFoundException($"Adapter '{adapterName}' not found")
    };

    private async Task AddAdapterStatusAsync(List<AdapterStatusDto> adapters, string name, IMesAdapter adapter)
    {
        var info = adapter.GetAdapterInfo();
        var health = await adapter.HealthCheckAsync();
        adapters.Add(new AdapterStatusDto
        {
            Name = name,
            Info = info,
            HealthStatus = health.Data,
            IsHealthy = health.Data?.IsHealthy ?? false
        });
    }
}

public class AdapterStatusDto
{
    public string Name { get; set; } = string.Empty;
    public AdapterInfo Info { get; set; } = new();
    public HealthStatus? HealthStatus { get; set; }
    public bool IsHealthy { get; set; }
}
