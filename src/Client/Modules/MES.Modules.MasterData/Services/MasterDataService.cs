using MES.Infrastructure.Persistence;
using MES.Infrastructure.Persistence.Entities;
using MES.Infrastructure.Persistence.Repositories;

namespace MES.Modules.MasterData.Services;

public interface IMasterDataService
{
    Task<List<ProductInfo>> GetAllProductsAsync();
    Task SaveProductAsync(ProductInfo product);
    Task DeleteProductAsync(string productId);

    Task<List<RouteInfo>> GetAllRoutesAsync();
    Task SaveRouteAsync(RouteInfo route);
    Task DeleteRouteAsync(string routeId);

    Task<List<RecipeInfo>> GetAllRecipesAsync();
    Task SaveRecipeAsync(RecipeInfo recipe);
    Task DeleteRecipeAsync(string recipeId);

    Task<List<EquipmentInfo>> GetAllEquipmentsAsync();
    Task SaveEquipmentAsync(EquipmentInfo equipment);
    Task DeleteEquipmentAsync(string equipmentId);
    Task UpdateEquipmentStatusAsync(string equipmentId, string status);

    Task<List<MaterialInfo>> GetAllMaterialsAsync();
    Task SaveMaterialAsync(MaterialInfo material);
    Task DeleteMaterialAsync(string materialId);

    Task<List<CustomerInfo>> GetAllCustomersAsync();
    Task SaveCustomerAsync(CustomerInfo customer);
    Task DeleteCustomerAsync(string customerId);

    Task<List<ReasonCodeInfo>> GetAllReasonCodesAsync();
    Task SaveReasonCodeAsync(ReasonCodeInfo reasonCode);
    Task DeleteReasonCodeAsync(string reasonCodeId);

    Task<List<DefectCodeInfo>> GetAllDefectCodesAsync();
    Task SaveDefectCodeAsync(DefectCodeInfo defectCode);
    Task DeleteDefectCodeAsync(string defectCodeId);

    Task<List<CarrierInfo>> GetAllCarriersAsync();
    Task SaveCarrierAsync(CarrierInfo carrier);
    Task DeleteCarrierAsync(string carrierId);
    Task UpdateCarrierStatusAsync(string carrierId, string status);

    Task<List<YieldRuleInfo>> GetAllYieldRulesAsync();
    Task SaveYieldRuleAsync(YieldRuleInfo rule);
    Task DeleteYieldRuleAsync(string ruleId);

    Task<List<ScrapRuleInfo>> GetAllScrapRulesAsync();
    Task SaveScrapRuleAsync(ScrapRuleInfo rule);
    Task DeleteScrapRuleAsync(string ruleId);

    Task<List<AlarmRuleInfo>> GetAllAlarmRulesAsync();
    Task SaveAlarmRuleAsync(AlarmRuleInfo rule);
    Task DeleteAlarmRuleAsync(string ruleId);
}

public class ProductInfo
{
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string ProductType { get; set; } = string.Empty;
    public string PackageType { get; set; } = string.Empty;
    public string DieName { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public string CustomerPN { get; set; } = string.Empty;
    public string InternalPN { get; set; } = string.Empty;
    public string Status { get; set; } = "Active";
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class RouteInfo
{
    public string RouteId { get; set; } = string.Empty;
    public string RouteName { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public string Version { get; set; } = "1.0";
    public string Status { get; set; } = "Active";
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class RecipeInfo
{
    public string RecipeId { get; set; } = string.Empty;
    public string RecipeName { get; set; } = string.Empty;
    public string EquipmentId { get; set; } = string.Empty;
    public string StepCode { get; set; } = string.Empty;
    public string Status { get; set; } = "Active";
    public bool IsActive { get; set; } = true;
    public string? Parameters { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class EquipmentInfo
{
    public string EquipmentId { get; set; } = string.Empty;
    public string EquipmentName { get; set; } = string.Empty;
    public string EquipmentType { get; set; } = string.Empty;
    public string EquipmentGroup { get; set; } = string.Empty;
    public string Status { get; set; } = "Available";
    public string? Location { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class MaterialInfo
{
    public string MaterialId { get; set; } = string.Empty;
    public string MaterialName { get; set; } = string.Empty;
    public string MaterialType { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public string Status { get; set; } = "Active";
    public string? Supplier { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CustomerInfo
{
    public string CustomerId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string ContactPerson { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Status { get; set; } = "Active";
    public string? Address { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ReasonCodeInfo
{
    public string ReasonCodeId { get; set; } = string.Empty;
    public string ReasonCode { get; set; } = string.Empty;
    public string ReasonName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Status { get; set; } = "Active";
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class DefectCodeInfo
{
    public string DefectCodeId { get; set; } = string.Empty;
    public string DefectCode { get; set; } = string.Empty;
    public string DefectName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Severity { get; set; } = "Minor";
    public string Status { get; set; } = "Active";
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CarrierInfo
{
    public string CarrierId { get; set; } = string.Empty;
    public string CarrierName { get; set; } = string.Empty;
    public string CarrierType { get; set; } = string.Empty;
    public string Status { get; set; } = "Available";
    public int Capacity { get; set; }
    public string? Location { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class YieldRuleInfo
{
    public string RuleId { get; set; } = string.Empty;
    public string RuleName { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public string StepCode { get; set; } = string.Empty;
    public double TargetYield { get; set; }
    public double WarningYield { get; set; }
    public string Status { get; set; } = "Active";
    public DateTime CreatedAt { get; set; }
}

public class ScrapRuleInfo
{
    public string RuleId { get; set; } = string.Empty;
    public string RuleName { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public string StepCode { get; set; } = string.Empty;
    public int MaxScrapQty { get; set; }
    public double MaxScrapRate { get; set; }
    public string Status { get; set; } = "Active";
    public DateTime CreatedAt { get; set; }
}

public class AlarmRuleInfo
{
    public string RuleId { get; set; } = string.Empty;
    public string RuleName { get; set; } = string.Empty;
    public string EquipmentId { get; set; } = string.Empty;
    public string AlarmType { get; set; } = string.Empty;
    public string Severity { get; set; } = "Warning";
    public string Condition { get; set; } = string.Empty;
    public string Status { get; set; } = "Active";
    public DateTime CreatedAt { get; set; }
}

public class MasterDataService : IMasterDataService
{
    private readonly IRepository<MasterProduct> _productRepo;
    private readonly IRepository<MasterRoute> _routeRepo;
    private readonly IRepository<MasterRecipe> _recipeRepo;
    private readonly IRepository<MasterEquipment> _equipRepo;
    private readonly IRepository<MasterMaterial> _materialRepo;
    private readonly IRepository<MasterCustomer> _customerRepo;
    private readonly IRepository<MasterReasonCode> _reasonCodeRepo;
    private readonly IRepository<MasterDefectCode> _defectCodeRepo;
    private readonly IRepository<MasterCarrier> _carrierRepo;
    private readonly IRepository<MasterYieldRule> _yieldRuleRepo;
    private readonly IRepository<MasterScrapRule> _scrapRuleRepo;
    private readonly IRepository<MasterAlarmRule> _alarmRuleRepo;

    public MasterDataService(
        IRepository<MasterProduct> productRepo,
        IRepository<MasterRoute> routeRepo,
        IRepository<MasterRecipe> recipeRepo,
        IRepository<MasterEquipment> equipRepo,
        IRepository<MasterMaterial> materialRepo,
        IRepository<MasterCustomer> customerRepo,
        IRepository<MasterReasonCode> reasonCodeRepo,
        IRepository<MasterDefectCode> defectCodeRepo,
        IRepository<MasterCarrier> carrierRepo,
        IRepository<MasterYieldRule> yieldRuleRepo,
        IRepository<MasterScrapRule> scrapRuleRepo,
        IRepository<MasterAlarmRule> alarmRuleRepo)
    {
        _productRepo = productRepo;
        _routeRepo = routeRepo;
        _recipeRepo = recipeRepo;
        _equipRepo = equipRepo;
        _materialRepo = materialRepo;
        _customerRepo = customerRepo;
        _reasonCodeRepo = reasonCodeRepo;
        _defectCodeRepo = defectCodeRepo;
        _carrierRepo = carrierRepo;
        _yieldRuleRepo = yieldRuleRepo;
        _scrapRuleRepo = scrapRuleRepo;
        _alarmRuleRepo = alarmRuleRepo;
    }

    public async Task<List<ProductInfo>> GetAllProductsAsync()
    {
        var entities = await _productRepo.GetAllAsync();
        return entities.Select(e => new ProductInfo
        {
            ProductId = e.ProductId,
            ProductName = e.ProductName,
            ProductType = e.ProcessStage ?? string.Empty,
            PackageType = e.PackageType ?? string.Empty,
            DieName = e.DieName ?? string.Empty,
            CustomerId = e.CustomerId ?? string.Empty,
            CustomerPN = e.CustomerPn ?? string.Empty,
            InternalPN = e.InternalPn ?? string.Empty,
            Status = e.Status ?? "Active",
            Description = e.DefaultRouteId,
            CreatedAt = e.CreatedAt,
        }).ToList();
    }

    public async Task SaveProductAsync(ProductInfo product)
    {
        var entity = new MasterProduct
        {
            ProductId = product.ProductId,
            ProductName = product.ProductName,
            PackageType = product.PackageType,
            DieName = product.DieName,
            ProcessStage = product.ProductType,
            CustomerId = product.CustomerId,
            CustomerPn = product.CustomerPN,
            InternalPn = product.InternalPN,
            Status = product.Status,
            DefaultRouteId = product.Description,
        };
        var exists = await _productRepo.ExistsAsync(p => p.ProductId == product.ProductId);
        if (exists) await _productRepo.UpdateAsync(entity);
        else await _productRepo.AddAsync(entity);
    }

    public async Task DeleteProductAsync(string productId)
    {
        var entity = await _productRepo.GetByIdAsync(productId);
        if (entity is not null) await _productRepo.DeleteAsync(entity);
    }

    public async Task<List<RouteInfo>> GetAllRoutesAsync()
    {
        var entities = await _routeRepo.GetAllAsync();
        return entities.Select(e => new RouteInfo
        {
            RouteId = e.RouteId,
            RouteName = e.RouteName,
            ProductId = e.ProductId,
            Version = e.RouteVersion,
            Status = e.IsActive ? "Active" : "Inactive",
            Description = e.PackageType ?? string.Empty,
            CreatedAt = e.CreatedAt,
        }).ToList();
    }

    public async Task SaveRouteAsync(RouteInfo route)
    {
        var entity = new MasterRoute
        {
            RouteId = route.RouteId,
            RouteName = route.RouteName,
            ProductId = route.ProductId,
            RouteVersion = route.Version,
            IsActive = route.Status == "Active",
            PackageType = route.Description,
        };
        var exists = await _routeRepo.ExistsAsync(r => r.RouteId == route.RouteId);
        if (exists) await _routeRepo.UpdateAsync(entity);
        else await _routeRepo.AddAsync(entity);
    }

    public async Task DeleteRouteAsync(string routeId)
    {
        var entity = await _routeRepo.GetByIdAsync(routeId);
        if (entity is not null) await _routeRepo.DeleteAsync(entity);
    }

    public async Task<List<RecipeInfo>> GetAllRecipesAsync()
    {
        var entities = await _recipeRepo.GetAllAsync();
        return entities.Select(e => new RecipeInfo
        {
            RecipeId = e.RecipeId,
            RecipeName = e.RecipeName,
            EquipmentId = e.EquipmentGroup,
            StepCode = e.StepCode,
            Status = e.IsActive ? "Active" : "Inactive",
            IsActive = e.IsActive,
            Parameters = e.Parameters,
            CreatedAt = e.CreatedAt,
        }).ToList();
    }

    public async Task SaveRecipeAsync(RecipeInfo recipe)
    {
        var entity = new MasterRecipe
        {
            RecipeId = recipe.RecipeId,
            RecipeName = recipe.RecipeName,
            EquipmentGroup = recipe.EquipmentId,
            StepCode = recipe.StepCode,
            IsActive = recipe.IsActive,
            Parameters = recipe.Parameters,
        };
        var exists = await _recipeRepo.ExistsAsync(r => r.RecipeId == recipe.RecipeId);
        if (exists) await _recipeRepo.UpdateAsync(entity);
        else await _recipeRepo.AddAsync(entity);
    }

    public async Task DeleteRecipeAsync(string recipeId)
    {
        var entity = await _recipeRepo.GetByIdAsync(recipeId);
        if (entity is not null) await _recipeRepo.DeleteAsync(entity);
    }

    public async Task<List<EquipmentInfo>> GetAllEquipmentsAsync()
    {
        var entities = await _equipRepo.GetAllAsync();
        return entities.Select(e => new EquipmentInfo
        {
            EquipmentId = e.EquipmentId,
            EquipmentName = e.EquipmentName,
            EquipmentType = e.EquipmentType ?? string.Empty,
            EquipmentGroup = e.EquipmentGroup ?? string.Empty,
            Status = e.Status ?? "Available",
            Location = e.Location,
            Description = e.Vendor ?? string.Empty,
            CreatedAt = e.CreatedAt,
        }).ToList();
    }

    public async Task SaveEquipmentAsync(EquipmentInfo equipment)
    {
        var entity = new MasterEquipment
        {
            EquipmentId = equipment.EquipmentId,
            EquipmentName = equipment.EquipmentName,
            EquipmentType = equipment.EquipmentType,
            EquipmentGroup = equipment.EquipmentGroup,
            Status = equipment.Status,
            Location = equipment.Location,
            Vendor = equipment.Description,
        };
        var exists = await _equipRepo.ExistsAsync(e => e.EquipmentId == equipment.EquipmentId);
        if (exists) await _equipRepo.UpdateAsync(entity);
        else await _equipRepo.AddAsync(entity);
    }

    public async Task DeleteEquipmentAsync(string equipmentId)
    {
        var entity = await _equipRepo.GetByIdAsync(equipmentId);
        if (entity is not null) await _equipRepo.DeleteAsync(entity);
    }

    public async Task UpdateEquipmentStatusAsync(string equipmentId, string status)
    {
        var entity = await _equipRepo.GetByIdAsync(equipmentId);
        if (entity is null) return;
        entity.Status = status;
        await _equipRepo.UpdateAsync(entity);
    }

    public async Task<List<MaterialInfo>> GetAllMaterialsAsync()
    {
        var entities = await _materialRepo.GetAllAsync();
        return entities.Select(e => new MaterialInfo
        {
            MaterialId = e.MaterialId,
            MaterialName = e.MaterialName,
            MaterialType = e.MaterialType ?? string.Empty,
            Unit = e.Unit ?? "pcs",
            Status = e.IsActive ? "Active" : "Inactive",
            Supplier = e.Supplier,
            Description = e.Specification,
            CreatedAt = e.CreatedAt,
        }).ToList();
    }

    public async Task SaveMaterialAsync(MaterialInfo material)
    {
        var entity = new MasterMaterial
        {
            MaterialId = material.MaterialId,
            MaterialName = material.MaterialName,
            MaterialType = material.MaterialType,
            Unit = material.Unit,
            IsActive = material.Status == "Active",
            Supplier = material.Supplier,
            Specification = material.Description,
        };
        var exists = await _materialRepo.ExistsAsync(m => m.MaterialId == material.MaterialId);
        if (exists) await _materialRepo.UpdateAsync(entity);
        else await _materialRepo.AddAsync(entity);
    }

    public async Task DeleteMaterialAsync(string materialId)
    {
        var entity = await _materialRepo.GetByIdAsync(materialId);
        if (entity is not null) await _materialRepo.DeleteAsync(entity);
    }

    public async Task<List<CustomerInfo>> GetAllCustomersAsync()
    {
        var entities = await _customerRepo.GetAllAsync();
        return entities.Select(e => new CustomerInfo
        {
            CustomerId = e.CustomerId,
            CustomerName = e.CustomerName,
            ContactPerson = e.ContactPerson ?? string.Empty,
            Phone = e.ContactPhone ?? string.Empty,
            Email = e.Email ?? string.Empty,
            Status = e.Status ?? "Active",
            Address = e.Address,
            CreatedAt = e.CreatedAt,
        }).ToList();
    }

    public async Task SaveCustomerAsync(CustomerInfo customer)
    {
        var entity = new MasterCustomer
        {
            CustomerId = customer.CustomerId,
            CustomerName = customer.CustomerName,
            ContactPerson = customer.ContactPerson,
            ContactPhone = customer.Phone,
            Email = customer.Email,
            Status = customer.Status,
            Address = customer.Address,
        };
        var exists = await _customerRepo.ExistsAsync(c => c.CustomerId == customer.CustomerId);
        if (exists) await _customerRepo.UpdateAsync(entity);
        else await _customerRepo.AddAsync(entity);
    }

    public async Task DeleteCustomerAsync(string customerId)
    {
        var entity = await _customerRepo.GetByIdAsync(customerId);
        if (entity is not null) await _customerRepo.DeleteAsync(entity);
    }

    public async Task<List<ReasonCodeInfo>> GetAllReasonCodesAsync()
    {
        var entities = await _reasonCodeRepo.GetAllAsync();
        return entities.Select(e => new ReasonCodeInfo
        {
            ReasonCodeId = e.ReasonCodeId,
            ReasonCode = e.ReasonCodeId,
            ReasonName = e.ReasonText,
            Category = e.Category ?? string.Empty,
            Status = e.IsEnabled ? "Active" : "Inactive",
            Description = e.ApplicableTo,
            CreatedAt = e.CreatedAt,
        }).ToList();
    }

    public async Task SaveReasonCodeAsync(ReasonCodeInfo reasonCode)
    {
        var entity = new MasterReasonCode
        {
            ReasonCodeId = reasonCode.ReasonCodeId,
            Category = reasonCode.Category,
            ReasonText = reasonCode.ReasonName,
            ApplicableTo = reasonCode.Description ?? string.Empty,
            IsEnabled = reasonCode.Status == "Active",
        };
        var exists = await _reasonCodeRepo.ExistsAsync(r => r.ReasonCodeId == reasonCode.ReasonCodeId);
        if (exists) await _reasonCodeRepo.UpdateAsync(entity);
        else await _reasonCodeRepo.AddAsync(entity);
    }

    public async Task DeleteReasonCodeAsync(string reasonCodeId)
    {
        var entity = await _reasonCodeRepo.GetByIdAsync(reasonCodeId);
        if (entity is not null) await _reasonCodeRepo.DeleteAsync(entity);
    }

    public async Task<List<DefectCodeInfo>> GetAllDefectCodesAsync()
    {
        var entities = await _defectCodeRepo.GetAllAsync();
        return entities.Select(e => new DefectCodeInfo
        {
            DefectCodeId = e.DefectCodeId,
            DefectCode = e.DefectCodeId,
            DefectName = e.DefectText,
            Category = e.DefectCategory ?? string.Empty,
            Severity = e.Severity ?? "Major",
            Status = e.IsEnabled ? "Active" : "Inactive",
            Description = string.Empty,
            CreatedAt = e.CreatedAt,
        }).ToList();
    }

    public async Task SaveDefectCodeAsync(DefectCodeInfo defectCode)
    {
        var entity = new MasterDefectCode
        {
            DefectCodeId = defectCode.DefectCodeId,
            DefectCategory = defectCode.Category,
            DefectText = defectCode.DefectName,
            Severity = defectCode.Severity,
            IsEnabled = defectCode.Status == "Active",
        };
        var exists = await _defectCodeRepo.ExistsAsync(d => d.DefectCodeId == defectCode.DefectCodeId);
        if (exists) await _defectCodeRepo.UpdateAsync(entity);
        else await _defectCodeRepo.AddAsync(entity);
    }

    public async Task DeleteDefectCodeAsync(string defectCodeId)
    {
        var entity = await _defectCodeRepo.GetByIdAsync(defectCodeId);
        if (entity is not null) await _defectCodeRepo.DeleteAsync(entity);
    }

    public async Task<List<CarrierInfo>> GetAllCarriersAsync()
    {
        var entities = await _carrierRepo.GetAllAsync();
        return entities.Select(e => new CarrierInfo
        {
            CarrierId = e.CarrierId,
            CarrierName = e.CarrierId,
            CarrierType = e.CarrierType ?? string.Empty,
            Status = e.Status ?? "Available",
            Capacity = e.Capacity,
            Location = e.Location,
            CreatedAt = e.CreatedAt,
        }).ToList();
    }

    public async Task SaveCarrierAsync(CarrierInfo carrier)
    {
        var entity = new MasterCarrier
        {
            CarrierId = carrier.CarrierId,
            CarrierType = carrier.CarrierType,
            Status = carrier.Status,
            Capacity = carrier.Capacity,
            Location = carrier.Location,
        };
        var exists = await _carrierRepo.ExistsAsync(c => c.CarrierId == carrier.CarrierId);
        if (exists) await _carrierRepo.UpdateAsync(entity);
        else await _carrierRepo.AddAsync(entity);
    }

    public async Task DeleteCarrierAsync(string carrierId)
    {
        var entity = await _carrierRepo.GetByIdAsync(carrierId);
        if (entity is not null) await _carrierRepo.DeleteAsync(entity);
    }

    public async Task UpdateCarrierStatusAsync(string carrierId, string status)
    {
        var entity = await _carrierRepo.GetByIdAsync(carrierId);
        if (entity is null) return;
        entity.Status = status;
        await _carrierRepo.UpdateAsync(entity);
    }

    public async Task<List<YieldRuleInfo>> GetAllYieldRulesAsync()
    {
        var entities = await _yieldRuleRepo.GetAllAsync();
        return entities.Select(e => new YieldRuleInfo
        {
            RuleId = e.RuleId,
            RuleName = $"{e.RouteId}-{e.StepCode}",
            ProductId = string.Empty,
            StepCode = e.StepCode,
            TargetYield = (double)e.YieldThreshold,
            WarningYield = (double)e.YieldThreshold * 0.9,
            Status = e.IsActive ? "Active" : "Inactive",
            CreatedAt = e.CreatedAt,
        }).ToList();
    }

    public async Task SaveYieldRuleAsync(YieldRuleInfo rule)
    {
        var entity = new MasterYieldRule
        {
            RuleId = rule.RuleId,
            RouteId = rule.StepCode.Contains('-') ? rule.StepCode.Split('-')[0] : string.Empty,
            StepCode = rule.StepCode,
            YieldThreshold = (decimal)rule.TargetYield,
            IsActive = rule.Status == "Active",
        };
        var exists = await _yieldRuleRepo.ExistsAsync(r => r.RuleId == rule.RuleId);
        if (exists) await _yieldRuleRepo.UpdateAsync(entity);
        else await _yieldRuleRepo.AddAsync(entity);
    }

    public async Task DeleteYieldRuleAsync(string ruleId)
    {
        var entity = await _yieldRuleRepo.GetByIdAsync(ruleId);
        if (entity is not null) await _yieldRuleRepo.DeleteAsync(entity);
    }

    public async Task<List<ScrapRuleInfo>> GetAllScrapRulesAsync()
    {
        var entities = await _scrapRuleRepo.GetAllAsync();
        return entities.Select(e => new ScrapRuleInfo
        {
            RuleId = e.RuleId,
            RuleName = $"{e.RouteId ?? ""}-{e.StepCode ?? ""}",
            ProductId = string.Empty,
            StepCode = e.StepCode ?? string.Empty,
            MaxScrapQty = 0,
            MaxScrapRate = (double)e.ThresholdPercent,
            Status = e.IsActive ? "Active" : "Inactive",
            CreatedAt = e.CreatedAt,
        }).ToList();
    }

    public async Task SaveScrapRuleAsync(ScrapRuleInfo rule)
    {
        var entity = new MasterScrapRule
        {
            RuleId = rule.RuleId,
            RouteId = rule.StepCode.Contains('-') ? rule.StepCode.Split('-')[0] : null,
            StepCode = rule.StepCode,
            ThresholdPercent = (decimal)rule.MaxScrapRate,
            IsActive = rule.Status == "Active",
        };
        var exists = await _scrapRuleRepo.ExistsAsync(r => r.RuleId == rule.RuleId);
        if (exists) await _scrapRuleRepo.UpdateAsync(entity);
        else await _scrapRuleRepo.AddAsync(entity);
    }

    public async Task DeleteScrapRuleAsync(string ruleId)
    {
        var entity = await _scrapRuleRepo.GetByIdAsync(ruleId);
        if (entity is not null) await _scrapRuleRepo.DeleteAsync(entity);
    }

    public async Task<List<AlarmRuleInfo>> GetAllAlarmRulesAsync()
    {
        var entities = await _alarmRuleRepo.GetAllAsync();
        return entities.Select(e => new AlarmRuleInfo
        {
            RuleId = e.RuleId,
            RuleName = $"{e.AlarmType}-{e.Severity}",
            EquipmentId = string.Empty,
            AlarmType = e.AlarmType,
            Severity = e.Severity ?? "Warning",
            Condition = string.Empty,
            Status = e.IsEnabled ? "Active" : "Inactive",
            CreatedAt = e.CreatedAt,
        }).ToList();
    }

    public async Task SaveAlarmRuleAsync(AlarmRuleInfo rule)
    {
        var entity = new MasterAlarmRule
        {
            RuleId = rule.RuleId,
            AlarmType = rule.AlarmType,
            Severity = rule.Severity,
            IsEnabled = rule.Status == "Active",
        };
        var exists = await _alarmRuleRepo.ExistsAsync(r => r.RuleId == rule.RuleId);
        if (exists) await _alarmRuleRepo.UpdateAsync(entity);
        else await _alarmRuleRepo.AddAsync(entity);
    }

    public async Task DeleteAlarmRuleAsync(string ruleId)
    {
        var entity = await _alarmRuleRepo.GetByIdAsync(ruleId);
        if (entity is not null) await _alarmRuleRepo.DeleteAsync(entity);
    }
}
