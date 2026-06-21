using MES.Adapters.Abstractions;

namespace MES.Adapters.Abstractions.Qms;

/// <summary>
/// QMS 质量管理系统对接适配器
/// 对接场景：MES→QMS：检验数据、不合格品信息、SPC 数据
///          QMS→MES：检验标准、FMEA 信息、质量 Alert
/// </summary>
public interface IMesQmsAdapter : IMesAdapter
{
    // MES → QMS 推送
    Task<AdapterResult<InspectionPushResult>> PushInspectionDataAsync(InspectionData data);
    Task<AdapterResult<NonconformingResult>> PushNonconformingInfoAsync(NonconformingData data);
    Task<AdapterResult<SpcPushResult>> PushSpcDataAsync(SpcData data);

    // QMS → MES 拉取
    Task<AdapterResult<InspectionStandard>> PullInspectionStandardAsync(StandardQuery query);
    Task<AdapterResult<FmeaData>> PullFmeaDataAsync(FmeaQuery query);
    Task<AdapterResult<QualityAlert>> ReceiveQualityAlertAsync();
}

public class InspectionData
{
    public string InspectionId { get; set; } = string.Empty;
    public string InspectionType { get; set; } = string.Empty; // IQC, IPQC, FQC, OQC
    public string LotId { get; set; } = string.Empty;
    public string? MaterialId { get; set; }
    public string? ProductId { get; set; }
    public List<InspectionItemResult> Items { get; set; } = new();
    public string? InspectorId { get; set; }
    public DateTime InspectionTime { get; set; }
}

public class InspectionItemResult
{
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public decimal? MeasuredValue { get; set; }
    public string? LowerLimit { get; set; }
    public string? UpperLimit { get; set; }
    public string Judgment { get; set; } = string.Empty; // Pass, Fail
}

public class InspectionPushResult
{
    public string InspectionId { get; set; } = string.Empty;
    public bool Pushed { get; set; }
}

public class NonconformingData
{
    public string RecordId { get; set; } = string.Empty;
    public string LotId { get; set; } = string.Empty;
    public string? WorkOrderId { get; set; }
    public string NonconformingType { get; set; } = string.Empty;
    public string DefectCode { get; set; } = string.Empty;
    public string DefectDescription { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string? Disposition { get; set; } // Rework, Scrap, Concession
    public DateTime DiscoveredTime { get; set; }
}

public class NonconformingResult
{
    public string RecordId { get; set; } = string.Empty;
    public string? QmsRecordNo { get; set; }
}

public class SpcData
{
    public string EquipmentId { get; set; } = string.Empty;
    public string ProcessCode { get; set; } = string.Empty;
    public string ParameterName { get; set; } = string.Empty;
    public List<SpcMeasurement> Measurements { get; set; } = new();
}

public class SpcMeasurement
{
    public DateTime MeasurementTime { get; set; }
    public decimal Value { get; set; }
    public string? LotId { get; set; }
    public string? SubGroup { get; set; }
}

public class SpcPushResult
{
    public int PushedCount { get; set; }
}

public class StandardQuery
{
    public string? MaterialId { get; set; }
    public string? ProductId { get; set; }
    public string? InspectionType { get; set; }
}

public class InspectionStandard
{
    public string StandardId { get; set; } = string.Empty;
    public string MaterialId { get; set; } = string.Empty;
    public string InspectionType { get; set; } = string.Empty;
    public List<InspectionItemStandard> Items { get; set; } = new();
    public string? Version { get; set; }
    public DateTime? EffectiveDate { get; set; }
}

public class InspectionItemStandard
{
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public string TestMethod { get; set; } = string.Empty;
    public string? TargetValue { get; set; }
    public string? LowerLimit { get; set; }
    public string? UpperLimit { get; set; }
    public string? SamplingPlan { get; set; }
}

public class FmeaQuery
{
    public string? ProcessCode { get; set; }
    public string? ProductId { get; set; }
}

public class FmeaData
{
    public string FmeaId { get; set; } = string.Empty;
    public string ProcessCode { get; set; } = string.Empty;
    public List<FmeaItem> Items { get; set; } = new();
}

public class FmeaItem
{
    public string FailureMode { get; set; } = string.Empty;
    public string FailureEffect { get; set; } = string.Empty;
    public string FailureCause { get; set; } = string.Empty;
    public int Severity { get; set; }
    public int Occurrence { get; set; }
    public int Detection { get; set; }
    public int Rpn { get; set; }
    public string? ControlMeasure { get; set; }
}

public class QualityAlert
{
    public string AlertId { get; set; } = string.Empty;
    public string AlertType { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty; // Critical, Major, Minor
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> AffectedProducts { get; set; } = new();
    public List<string> AffectedLots { get; set; } = new();
    public DateTime AlertTime { get; set; }
    public string? IssuedBy { get; set; }
}
