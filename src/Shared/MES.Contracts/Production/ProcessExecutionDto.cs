namespace MES.Contracts.Production;

/// <summary>
/// 工序启动请求
/// </summary>
public class StepStartRequest
{
    /// <summary>
    /// 批次ID
    /// </summary>
    public string LotId { get; set; } = string.Empty;

    /// <summary>
    /// 工序代码
    /// </summary>
    public string StepCode { get; set; } = string.Empty;

    /// <summary>
    /// 设备ID
    /// </summary>
    public string EquipmentId { get; set; } = string.Empty;

    /// <summary>
    /// 操作员ID
    /// </summary>
    public string OperatorId { get; set; } = string.Empty;

    /// <summary>
    /// 工艺参数列表
    /// </summary>
    public List<StepParameterRecord> Parameters { get; set; } = [];
}

/// <summary>
/// 工序完成请求
/// </summary>
public class StepCompleteRequest
{
    /// <summary>
    /// 批次ID
    /// </summary>
    public string LotId { get; set; } = string.Empty;

    /// <summary>
    /// 工序代码
    /// </summary>
    public string StepCode { get; set; } = string.Empty;

    /// <summary>
    /// 合格数量
    /// </summary>
    public int PassQty { get; set; }

    /// <summary>
    /// 不合格数量
    /// </summary>
    public int FailQty { get; set; }

    /// <summary>
    /// 报废数量
    /// </summary>
    public int ScrapQty { get; set; }

    /// <summary>
    /// 操作员ID
    /// </summary>
    public string OperatorId { get; set; } = string.Empty;

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remarks { get; set; }
}

/// <summary>
/// 工艺参数记录
/// </summary>
public class StepParameterRecord
{
    /// <summary>
    /// 参数名称
    /// </summary>
    public string ParameterName { get; set; } = string.Empty;

    /// <summary>
    /// 参数值
    /// </summary>
    public string ParameterValue { get; set; } = string.Empty;

    /// <summary>
    /// 单位
    /// </summary>
    public string? Unit { get; set; }

    /// <summary>
    /// 规格上限
    /// </summary>
    public decimal? Usl { get; set; }

    /// <summary>
    /// 规格下限
    /// </summary>
    public decimal? Lsl { get; set; }
}

/// <summary>
/// 工序执行响应
/// </summary>
public class StepExecutionResponse
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 操作ID
    /// </summary>
    public string OperationId { get; set; } = string.Empty;

    /// <summary>
    /// 消息
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 当前工序代码
    /// </summary>
    public string CurrentStepCode { get; set; } = string.Empty;

    /// <summary>
    /// 当前工序序号
    /// </summary>
    public int CurrentStepSeq { get; set; }

    /// <summary>
    /// 下一工序代码
    /// </summary>
    public string NextStepCode { get; set; } = string.Empty;

    /// <summary>
    /// 下一工序序号
    /// </summary>
    public int NextStepSeq { get; set; }

    /// <summary>
    /// 是否最后一道工序
    /// </summary>
    public bool IsLastStep { get; set; }
}

/// <summary>
/// 工序状态响应
/// </summary>
public class StepStatusResponse
{
    /// <summary>
    /// 批次ID
    /// </summary>
    public string LotId { get; set; } = string.Empty;

    /// <summary>
    /// 工序代码
    /// </summary>
    public string StepCode { get; set; } = string.Empty;

    /// <summary>
    /// 工序序号
    /// </summary>
    public int StepSeq { get; set; }

    /// <summary>
    /// 工序状态 (Pending, InProduction, Completed, Skipped)
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// 进站时间
    /// </summary>
    public DateTime? TrackInTime { get; set; }

    /// <summary>
    /// 出站时间
    /// </summary>
    public DateTime? TrackOutTime { get; set; }

    /// <summary>
    /// 设备ID
    /// </summary>
    public string? EquipmentId { get; set; }

    /// <summary>
    /// 操作员ID
    /// </summary>
    public string? OperatorId { get; set; }

    /// <summary>
    /// 投入数量
    /// </summary>
    public int InputQty { get; set; }

    /// <summary>
    /// 合格数量
    /// </summary>
    public int PassQty { get; set; }

    /// <summary>
    /// 不合格数量
    /// </summary>
    public int FailQty { get; set; }

    /// <summary>
    /// 报废数量
    /// </summary>
    public int ScrapQty { get; set; }
}

/// <summary>
/// 工艺参数记录请求
/// </summary>
public class RecordParametersRequest
{
    /// <summary>
    /// 批次ID
    /// </summary>
    public string LotId { get; set; } = string.Empty;

    /// <summary>
    /// 工序代码
    /// </summary>
    public string StepCode { get; set; } = string.Empty;

    /// <summary>
    /// 参数列表
    /// </summary>
    public List<StepParameterRecord> Parameters { get; set; } = [];
}
