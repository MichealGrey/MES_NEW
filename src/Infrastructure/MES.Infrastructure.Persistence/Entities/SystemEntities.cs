namespace MES.Infrastructure.Persistence.Entities;

public class SysDepartment
{
    public string DeptId { get; set; } = string.Empty;
    public string DeptName { get; set; } = string.Empty;
    public string? ParentId { get; set; }
    public string? ManagerId { get; set; }
    public string Status { get; set; } = "Active";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class SysRole
{
    public string RoleId { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Level { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class SysUser
{
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string? PasswordHash { get; set; }
    public string RoleId { get; set; } = string.Empty;
    public string DeptId { get; set; } = string.Empty;
    public string? Shift { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class SysUserPermission
{
    public long Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string PermissionCode { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class SysSignatureLevel
{
    public string LevelCode { get; set; } = string.Empty;
    public string LevelName { get; set; } = string.Empty;
    public int LevelOrder { get; set; }
    public string? Description { get; set; }
}

public class ExtSystemEvent
{
    public string EventId { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public string SourceSystem { get; set; } = "MES";
    public string TargetSystem { get; set; } = string.Empty;
    public string? Payload { get; set; }
    public string Status { get; set; } = "Pending";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? SentAt { get; set; }
    public string? ErrorMessage { get; set; }
    public int RetryCount { get; set; }
}

public class ExtSystemConfig
{
    public string SystemId { get; set; } = string.Empty;
    public string SystemName { get; set; } = string.Empty;
    public string SystemType { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
    public string AuthType { get; set; } = "None";
    public string? AuthCredential { get; set; }
    public bool IsEnabled { get; set; } = true;
    public int TimeoutSeconds { get; set; } = 30;
    public int MaxRetries { get; set; } = 3;
    public string? SubscribedEvents { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class CustomerRequirement
{
    public string RequirementId { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string? OrderId { get; set; }
    public string? ProductId { get; set; }
    public string RequirementType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Priority { get; set; } = "Normal";
    public bool IsMandatory { get; set; }
    public string? VerificationMethod { get; set; }
    public string Status { get; set; } = "Active";
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
}

public class MasterMaterial
{
    public string MaterialId { get; set; } = string.Empty;
    public string MaterialName { get; set; } = string.Empty;
    public string MaterialType { get; set; } = string.Empty;
    public string? Specification { get; set; }
    public string Unit { get; set; } = "pcs";
    public string? Supplier { get; set; }
    public int MinStock { get; set; }
    public int CurrentStock { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class MaterialRequirement
{
    public string RequirementId { get; set; } = string.Empty;
    public string StepCode { get; set; } = string.Empty;
    public string MaterialId { get; set; } = string.Empty;
    public decimal RequiredQty { get; set; }
    public string Unit { get; set; } = "pcs";
    public bool IsMandatory { get; set; } = true;
}

public class MaterialConsumeEntity
{
    public string ConsumeId { get; set; } = string.Empty;
    public string LotId { get; set; } = string.Empty;
    public string StepCode { get; set; } = string.Empty;
    public string MaterialId { get; set; } = string.Empty;
    public string MaterialName { get; set; } = string.Empty;
    public decimal ConsumedQty { get; set; }
    public string Unit { get; set; } = "pcs";
    public string? BatchNo { get; set; }
    public string OperatorId { get; set; } = string.Empty;
    public DateTime ConsumedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class QualityGateEntity
{
    public string GateId { get; set; } = string.Empty;
    public string LotId { get; set; } = string.Empty;
    public string StepCode { get; set; } = string.Empty;
    public int StepSeq { get; set; }
    public string GateType { get; set; } = "QACheck";
    public string Status { get; set; } = "Pending";
    public string? CheckedBy { get; set; }
    public string? CheckedByName { get; set; }
    public DateTime? CheckedAt { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpireAt { get; set; }
}
