namespace MES.Modules.Production.Models;

public class RouteStep
{
    public string RouteId { get; set; } = string.Empty;
    public string RouteVersion { get; set; } = "1.0";
    public int StepSeq { get; set; }
    public string StepName { get; set; } = string.Empty;
    public string StepCode { get; set; } = string.Empty;

    public string EquipmentGroup { get; set; } = string.Empty;
    public string RequiredRecipeType { get; set; } = string.Empty;

    public bool RequireTrackIn { get; set; } = true;
    public bool RequireTrackOut { get; set; } = true;
    public bool RequireRecipeCheck { get; set; } = true;
    public bool RequireEquipmentCheck { get; set; } = true;
    public bool RequireMaterialCheck { get; set; }
    public bool RequireQualityGate { get; set; }
    public bool RequireQuantityBalance { get; set; } = true;

    public bool AllowSkip { get; set; }
    public bool AllowReworkToThisStep { get; set; }

    public int QueueTimeLimitMinutes { get; set; }

    // --- 良率控制 ---
    public int? YieldThreshold { get; set; }                  // 良率阈值(%)，低于则自动Hold

    // --- Split/Merge ---
    public bool RequireSplit { get; set; }                    // 是否必须拆批（如QA取样）
    public bool AllowMerge { get; set; }                      // 是否允许合批
    
    // --- 重工 ---
    public string? ReworkRouteId { get; set; }                // 关联重工路线ID
    public string? ReworkTargetStep { get; set; }             // 重工后回到哪一步（StepName）
    
    // --- 载具 ---
    public string RequiredCarrierType { get; set; } = string.Empty; // 必须载具类型: FOUP/TapeFrame/Magazine等
    
    // --- MRB ---
    public bool EnableMRB { get; set; }                       // 是否允许MRB
    public int? MRBThreshold { get; set; }                    // 不良超过多少触发MRB

    // --- 签核 ---
    public string? RequiredSignatureLevel { get; set; }       // "Level1"/"Level2"/"Level3"/"Level0"
}
