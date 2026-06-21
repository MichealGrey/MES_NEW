using MES.Infrastructure.Persistence;
using MES.Infrastructure.Persistence.Entities;
using MES.Infrastructure.Persistence.Repositories;
using MES.Modules.Production.Models;

namespace MES.Modules.Production.Services;

public class MasterDataService : IMasterDataService
{
    private readonly IRepository<MasterEquipment> _equipRepo;
    private readonly IRepository<MasterCarrier> _carrierRepo;
    private readonly IRepository<MasterRecipe> _recipeRepo;
    private readonly IRepository<SysUser> _userRepo;
    private readonly IRepository<MasterProduct> _productRepo;
    private readonly IRepository<MasterRoute> _routeRepo;
    private readonly IRepository<MasterMaterial> _materialRepo;
    private readonly IRepository<MasterYieldRule> _yieldRuleRepo;
    private readonly IRepository<MasterAlarmRule> _alarmRuleRepo;
    private readonly IRepository<MasterScrapRule> _scrapRuleRepo;
    private readonly IRepository<MasterCustomer> _customerRepo;
    private readonly IRepository<MasterReasonCode> _reasonCodeRepo;
    private readonly IRepository<MasterDefectCode> _defectCodeRepo;

    public MasterDataService(
        IRepository<MasterEquipment> equipRepo,
        IRepository<MasterCarrier> carrierRepo,
        IRepository<MasterRecipe> recipeRepo,
        IRepository<SysUser> userRepo,
        IRepository<MasterProduct> productRepo,
        IRepository<MasterRoute> routeRepo,
        IRepository<MasterMaterial> materialRepo,
        IRepository<MasterYieldRule> yieldRuleRepo,
        IRepository<MasterAlarmRule> alarmRuleRepo,
        IRepository<MasterScrapRule> scrapRuleRepo,
        IRepository<MasterCustomer> customerRepo,
        IRepository<MasterReasonCode> reasonCodeRepo,
        IRepository<MasterDefectCode> defectCodeRepo)
    {
        _equipRepo = equipRepo;
        _carrierRepo = carrierRepo;
        _recipeRepo = recipeRepo;
        _userRepo = userRepo;
        _productRepo = productRepo;
        _routeRepo = routeRepo;
        _materialRepo = materialRepo;
        _yieldRuleRepo = yieldRuleRepo;
        _alarmRuleRepo = alarmRuleRepo;
        _scrapRuleRepo = scrapRuleRepo;
        _customerRepo = customerRepo;
        _reasonCodeRepo = reasonCodeRepo;
        _defectCodeRepo = defectCodeRepo;
    }

    public async Task<List<EquipmentInfo>> GetAllEquipmentsAsync()
    {
        var entities = await _equipRepo.GetAllAsync();
        return entities.Select(MapToEquipmentInfo).ToList();
    }

    public async Task<EquipmentInfo?> GetEquipmentAsync(string equipmentId)
    {
        var entity = await _equipRepo.GetByIdAsync(equipmentId);
        return entity is null ? null : MapToEquipmentInfo(entity);
    }

    public async Task SaveEquipmentAsync(EquipmentInfo equipment)
    {
        var entity = MapToEquipmentEntity(equipment);
        var exists = await _equipRepo.ExistsAsync(e => e.EquipmentId == equipment.EquipmentId);
        if (exists)
            await _equipRepo.UpdateAsync(entity);
        else
            await _equipRepo.AddAsync(entity);
    }

    public async Task UpdateEquipmentStatusAsync(string equipmentId, string status)
    {
        var entity = await _equipRepo.GetByIdAsync(equipmentId);
        if (entity is null) return;
        entity.Status = status;
        await _equipRepo.UpdateAsync(entity);
    }

    public async Task<List<CarrierInfo>> GetAllCarriersAsync()
    {
        var entities = await _carrierRepo.GetAllAsync();
        return entities.Select(MapToCarrierInfo).ToList();
    }

    public async Task<CarrierInfo?> GetCarrierAsync(string carrierId)
    {
        var entity = await _carrierRepo.GetByIdAsync(carrierId);
        return entity is null ? null : MapToCarrierInfo(entity);
    }

    public async Task SaveCarrierAsync(CarrierInfo carrier)
    {
        var entity = MapToCarrierEntity(carrier);
        var exists = await _carrierRepo.ExistsAsync(c => c.CarrierId == carrier.CarrierId);
        if (exists)
            await _carrierRepo.UpdateAsync(entity);
        else
            await _carrierRepo.AddAsync(entity);
    }

    public async Task UpdateCarrierStatusAsync(string carrierId, string status)
    {
        var entity = await _carrierRepo.GetByIdAsync(carrierId);
        if (entity is null) return;
        entity.Status = status;
        await _carrierRepo.UpdateAsync(entity);
    }

    public async Task<List<RecipeInfo>> GetAllRecipesAsync()
    {
        var entities = await _recipeRepo.GetAllAsync();
        return entities.Select(MapToRecipeInfo).ToList();
    }

    public async Task<RecipeInfo?> GetRecipeAsync(string recipeId)
    {
        var entity = await _recipeRepo.GetByIdAsync(recipeId);
        return entity is null ? null : MapToRecipeInfo(entity);
    }

    public async Task SaveRecipeAsync(RecipeInfo recipe)
    {
        var entity = MapToRecipeEntity(recipe);
        var exists = await _recipeRepo.ExistsAsync(r => r.RecipeId == recipe.RecipeId);
        if (exists)
            await _recipeRepo.UpdateAsync(entity);
        else
            await _recipeRepo.AddAsync(entity);
    }

    public async Task<List<UserInfo>> GetAllUsersAsync()
    {
        var entities = await _userRepo.GetWhereAsync(u => u.IsActive);
        return entities.Select(MapToUserInfo).ToList();
    }

    public async Task<UserInfo?> GetUserAsync(string userId)
    {
        var entity = await _userRepo.GetByIdAsync(userId);
        return entity is null ? null : MapToUserInfo(entity);
    }

    public async Task SaveUserAsync(UserInfo user)
    {
        var entity = MapToUserEntity(user);
        var exists = await _userRepo.ExistsAsync(u => u.UserId == user.UserId);
        if (exists)
            await _userRepo.UpdateAsync(entity);
        else
            await _userRepo.AddAsync(entity);
    }

    public async Task<List<EquipmentInfo>> GetAvailableEquipmentsAsync(string equipmentGroup)
    {
        var entities = await _equipRepo.GetWhereAsync(e => e.EquipmentGroup == equipmentGroup && e.Status == "Available");
        return entities.Select(MapToEquipmentInfo).ToList();
    }

    public async Task<List<RecipeInfo>> GetRecipesForStepAsync(string equipmentGroup, string stepCode)
    {
        var entities = await _recipeRepo.GetWhereAsync(r => r.EquipmentGroup == equipmentGroup && r.StepCode == stepCode && r.IsActive);
        return entities.Select(MapToRecipeInfo).ToList();
    }

    // ========== Product CRUD ==========
    public async Task<List<ProductInfo>> GetAllProductsAsync()
    {
        var entities = await _productRepo.GetAllAsync();
        return entities.Select(MapToProductInfo).ToList();
    }

    public async Task<ProductInfo?> GetProductAsync(string productId)
    {
        var entity = await _productRepo.GetByIdAsync(productId);
        return entity is null ? null : MapToProductInfo(entity);
    }

    public async Task SaveProductAsync(ProductInfo product)
    {
        var entity = MapToProductEntity(product);
        var exists = await _productRepo.ExistsAsync(p => p.ProductId == product.ProductId);
        if (exists)
            await _productRepo.UpdateAsync(entity);
        else
            await _productRepo.AddAsync(entity);
    }

    public async Task DeleteProductAsync(string productId)
    {
        var entity = await _productRepo.GetByIdAsync(productId);
        if (entity is not null)
            await _productRepo.DeleteAsync(entity);
    }

    // ========== Route CRUD ==========
    public async Task<List<RouteInfo>> GetAllRoutesAsync()
    {
        var entities = await _routeRepo.GetAllAsync();
        return entities.Select(MapToRouteInfo).ToList();
    }

    public async Task<RouteInfo?> GetRouteAsync(string routeId)
    {
        var entity = await _routeRepo.GetByIdAsync(routeId);
        return entity is null ? null : MapToRouteInfo(entity);
    }

    public async Task SaveRouteAsync(RouteInfo route)
    {
        var entity = MapToRouteEntity(route);
        var exists = await _routeRepo.ExistsAsync(r => r.RouteId == route.RouteId);
        if (exists)
            await _routeRepo.UpdateAsync(entity);
        else
            await _routeRepo.AddAsync(entity);
    }

    public async Task DeleteRouteAsync(string routeId)
    {
        var entity = await _routeRepo.GetByIdAsync(routeId);
        if (entity is not null)
            await _routeRepo.DeleteAsync(entity);
    }

    // ========== Material CRUD ==========
    public async Task<List<MaterialInfo>> GetAllMaterialsAsync()
    {
        var entities = await _materialRepo.GetAllAsync();
        return entities.Select(MapToMaterialInfo).ToList();
    }

    public async Task<MaterialInfo?> GetMaterialAsync(string materialId)
    {
        var entity = await _materialRepo.GetByIdAsync(materialId);
        return entity is null ? null : MapToMaterialInfo(entity);
    }

    public async Task SaveMaterialAsync(MaterialInfo material)
    {
        var entity = MapToMaterialEntity(material);
        var exists = await _materialRepo.ExistsAsync(m => m.MaterialId == material.MaterialId);
        if (exists)
            await _materialRepo.UpdateAsync(entity);
        else
            await _materialRepo.AddAsync(entity);
    }

    public async Task DeleteMaterialAsync(string materialId)
    {
        var entity = await _materialRepo.GetByIdAsync(materialId);
        if (entity is not null)
            await _materialRepo.DeleteAsync(entity);
    }

    // ========== YieldRule CRUD ==========
    public async Task<List<YieldRule>> GetAllYieldRulesAsync()
    {
        var entities = await _yieldRuleRepo.GetAllAsync();
        return entities.Select(MapToYieldRule).ToList();
    }

    public async Task SaveYieldRuleAsync(YieldRule rule)
    {
        var entity = MapToYieldRuleEntity(rule);
        var exists = await _yieldRuleRepo.ExistsAsync(y => y.RuleId == rule.RuleId);
        if (exists)
            await _yieldRuleRepo.UpdateAsync(entity);
        else
            await _yieldRuleRepo.AddAsync(entity);
    }

    public async Task DeleteYieldRuleAsync(string ruleId)
    {
        var entity = await _yieldRuleRepo.GetByIdAsync(ruleId);
        if (entity is not null)
            await _yieldRuleRepo.DeleteAsync(entity);
    }

    // ========== AlarmRule CRUD ==========
    public async Task<List<AlarmRule>> GetAllAlarmRulesAsync()
    {
        var entities = await _alarmRuleRepo.GetAllAsync();
        return entities.Select(MapToAlarmRule).ToList();
    }

    public async Task SaveAlarmRuleAsync(AlarmRule rule)
    {
        var entity = MapToAlarmRuleEntity(rule);
        var exists = await _alarmRuleRepo.ExistsAsync(a => a.RuleId == rule.RuleId);
        if (exists)
            await _alarmRuleRepo.UpdateAsync(entity);
        else
            await _alarmRuleRepo.AddAsync(entity);
    }

    public async Task DeleteAlarmRuleAsync(string ruleId)
    {
        var entity = await _alarmRuleRepo.GetByIdAsync(ruleId);
        if (entity is not null)
            await _alarmRuleRepo.DeleteAsync(entity);
    }

    // ========== ScrapRule CRUD ==========
    public async Task<List<ScrapRule>> GetAllScrapRulesAsync()
    {
        var entities = await _scrapRuleRepo.GetAllAsync();
        return entities.Select(MapToScrapRule).ToList();
    }

    public async Task SaveScrapRuleAsync(ScrapRule rule)
    {
        var entity = MapToScrapRuleEntity(rule);
        var exists = await _scrapRuleRepo.ExistsAsync(s => s.RuleId == rule.RuleId);
        if (exists)
            await _scrapRuleRepo.UpdateAsync(entity);
        else
            await _scrapRuleRepo.AddAsync(entity);
    }

    public async Task DeleteScrapRuleAsync(string ruleId)
    {
        var entity = await _scrapRuleRepo.GetByIdAsync(ruleId);
        if (entity is not null)
            await _scrapRuleRepo.DeleteAsync(entity);
    }

    // ========== Customer CRUD ==========
    public async Task<List<CustomerInfo>> GetAllCustomersAsync()
    {
        var entities = await _customerRepo.GetAllAsync();
        return entities.Select(MapToCustomerInfo).ToList();
    }

    public async Task<CustomerInfo?> GetCustomerAsync(string customerId)
    {
        var entity = await _customerRepo.GetByIdAsync(customerId);
        return entity is null ? null : MapToCustomerInfo(entity);
    }

    public async Task SaveCustomerAsync(CustomerInfo customer)
    {
        var entity = MapToCustomerEntity(customer);
        var exists = await _customerRepo.ExistsAsync(c => c.CustomerId == customer.CustomerId);
        if (exists)
            await _customerRepo.UpdateAsync(entity);
        else
            await _customerRepo.AddAsync(entity);
    }

    public async Task UpdateCustomerStatusAsync(string customerId, string status)
    {
        var entity = await _customerRepo.GetByIdAsync(customerId);
        if (entity is null) return;
        entity.Status = status;
        await _customerRepo.UpdateAsync(entity);
    }

    // ========== ReasonCode CRUD ==========
    public async Task<List<ReasonCodeInfo>> GetAllReasonCodesAsync()
    {
        var entities = await _reasonCodeRepo.GetAllAsync();
        return entities.Select(MapToReasonCodeInfo).ToList();
    }

    public async Task SaveReasonCodeAsync(ReasonCodeInfo code)
    {
        var entity = MapToReasonCodeEntity(code);
        var exists = await _reasonCodeRepo.ExistsAsync(r => r.ReasonCodeId == code.ReasonCodeId);
        if (exists)
            await _reasonCodeRepo.UpdateAsync(entity);
        else
            await _reasonCodeRepo.AddAsync(entity);
    }

    public async Task DeleteReasonCodeAsync(string codeId)
    {
        var entity = await _reasonCodeRepo.GetByIdAsync(codeId);
        if (entity is not null)
            await _reasonCodeRepo.DeleteAsync(entity);
    }

    // ========== DefectCode CRUD ==========
    public async Task<List<DefectCodeInfo>> GetAllDefectCodesAsync()
    {
        var entities = await _defectCodeRepo.GetAllAsync();
        return entities.Select(MapToDefectCodeInfo).ToList();
    }

    public async Task SaveDefectCodeAsync(DefectCodeInfo code)
    {
        var entity = MapToDefectCodeEntity(code);
        var exists = await _defectCodeRepo.ExistsAsync(d => d.DefectCodeId == code.DefectCodeId);
        if (exists)
            await _defectCodeRepo.UpdateAsync(entity);
        else
            await _defectCodeRepo.AddAsync(entity);
    }

    public async Task DeleteDefectCodeAsync(string codeId)
    {
        var entity = await _defectCodeRepo.GetByIdAsync(codeId);
        if (entity is not null)
            await _defectCodeRepo.DeleteAsync(entity);
    }

    private static EquipmentInfo MapToEquipmentInfo(MasterEquipment e) => new()
    {
        EquipmentId = e.EquipmentId,
        EquipmentName = e.EquipmentName,
        EquipmentGroup = e.EquipmentGroup,
        EquipmentType = e.EquipmentType,
        ProcessStage = e.ProcessStage,
        Vendor = e.Vendor,
        Model = e.Model,
        SerialNumber = e.SerialNumber,
        Capability = e.Capability,
        Status = e.Status,
        CurrentLotId = e.CurrentLotId,
        CurrentRecipe = e.CurrentRecipe,
        SupportedRoutes = [],
        SupportedSteps = [],
        LastMaintenanceDate = e.LastMaintenanceDate ?? DateTime.MinValue,
        MaintenanceIntervalHours = e.MaintenanceIntervalHours,
        RunningHours = e.RunningHours,
        Location = e.Location ?? string.Empty,
        ResponsiblePerson = e.ResponsiblePerson ?? string.Empty,
        CreatedAt = e.CreatedAt,
    };

    private static MasterEquipment MapToEquipmentEntity(EquipmentInfo m) => new()
    {
        EquipmentId = m.EquipmentId,
        EquipmentName = m.EquipmentName,
        EquipmentGroup = m.EquipmentGroup,
        EquipmentType = m.EquipmentType,
        ProcessStage = m.ProcessStage,
        Vendor = m.Vendor,
        Model = m.Model,
        SerialNumber = m.SerialNumber,
        Capability = m.Capability,
        Status = m.Status,
        CurrentLotId = m.CurrentLotId,
        CurrentRecipe = m.CurrentRecipe,
        LastMaintenanceDate = m.LastMaintenanceDate == DateTime.MinValue ? null : m.LastMaintenanceDate,
        MaintenanceIntervalHours = m.MaintenanceIntervalHours,
        RunningHours = m.RunningHours,
        Location = m.Location,
        ResponsiblePerson = m.ResponsiblePerson,
        CreatedAt = m.CreatedAt,
    };

    private static CarrierInfo MapToCarrierInfo(MasterCarrier c) => new()
    {
        CarrierId = c.CarrierId,
        CarrierType = c.CarrierType,
        Status = c.Status,
        CurrentLotId = c.CurrentLotId,
        Capacity = c.Capacity,
        UseCount = c.UseCount,
        MaxUseCount = c.MaxUseCount,
        LastCleanDate = c.LastCleanDate ?? DateTime.MinValue,
        CleanIntervalUses = c.CleanIntervalUses,
        Location = c.Location ?? string.Empty,
        ApplicableProcess = c.ApplicableProcess,
        ApplicablePackage = c.ApplicablePackage,
        CreatedAt = c.CreatedAt,
    };

    private static MasterCarrier MapToCarrierEntity(CarrierInfo m) => new()
    {
        CarrierId = m.CarrierId,
        CarrierType = m.CarrierType,
        Status = m.Status,
        CurrentLotId = m.CurrentLotId,
        Capacity = m.Capacity,
        UseCount = m.UseCount,
        MaxUseCount = m.MaxUseCount,
        LastCleanDate = m.LastCleanDate == DateTime.MinValue ? null : m.LastCleanDate,
        CleanIntervalUses = m.CleanIntervalUses,
        Location = m.Location,
        ApplicableProcess = m.ApplicableProcess,
        ApplicablePackage = m.ApplicablePackage,
        CreatedAt = m.CreatedAt,
    };

    private static RecipeInfo MapToRecipeInfo(MasterRecipe r) => new()
    {
        RecipeId = r.RecipeId,
        RecipeName = r.RecipeName,
        EquipmentGroup = r.EquipmentGroup,
        ProductId = r.ProductId,
        StepCode = r.StepCode,
        Version = r.Version,
        IsActive = r.IsActive,
        Parameters = r.Parameters,
        ApprovedBy = r.ApprovedBy ?? string.Empty,
        ApprovedAt = r.ApprovedAt ?? DateTime.MinValue,
        CreatedAt = r.CreatedAt,
    };

    private static MasterRecipe MapToRecipeEntity(RecipeInfo m) => new()
    {
        RecipeId = m.RecipeId,
        RecipeName = m.RecipeName,
        EquipmentGroup = m.EquipmentGroup,
        ProductId = m.ProductId,
        StepCode = m.StepCode,
        Version = m.Version,
        IsActive = m.IsActive,
        Parameters = m.Parameters,
        ApprovedBy = m.ApprovedBy,
        ApprovedAt = m.ApprovedAt == DateTime.MinValue ? null : m.ApprovedAt,
        CreatedAt = m.CreatedAt,
    };

    private static UserInfo MapToUserInfo(SysUser u) => new()
    {
        UserId = u.UserId,
        UserName = u.UserName,
        Role = u.RoleId,
        Department = u.DeptId,
        Shift = u.Shift ?? string.Empty,
        IsActive = u.IsActive,
        Permissions = [],
        AuthorizedRoutes = [],
        AuthorizedEquipments = [],
        CreatedAt = u.CreatedAt,
    };

    private static SysUser MapToUserEntity(UserInfo m) => new()
    {
        UserId = m.UserId,
        UserName = m.UserName,
        RoleId = m.Role,
        DeptId = m.Department,
        Shift = m.Shift,
        IsActive = m.IsActive,
        CreatedAt = m.CreatedAt,
        UpdatedAt = DateTime.UtcNow,
    };

    // ========== Product Mappings ==========
    private static ProductInfo MapToProductInfo(MasterProduct e) => new()
    {
        ProductId = e.ProductId,
        ProductName = e.ProductName,
        ProductCode = e.InternalPn ?? string.Empty,
        CustomerId = e.CustomerId,
        CustomerName = e.CustomerName,
        PackageType = e.PackageType,
        ProcessStage = e.ProcessStage,
        DefaultRouteId = e.DefaultRouteId,
        UnitQty = e.UnitQty,
        Status = e.Status,
        Description = e.DieName ?? string.Empty,
    };

    private static MasterProduct MapToProductEntity(ProductInfo m) => new()
    {
        ProductId = m.ProductId,
        ProductName = m.ProductName,
        DieName = m.Description,
        PackageType = m.PackageType,
        ProcessStage = m.ProcessStage,
        DefaultRouteId = m.DefaultRouteId,
        UnitQty = m.UnitQty,
        CustomerId = m.CustomerId,
        CustomerName = m.CustomerName,
        InternalPn = m.ProductCode,
        Status = m.Status,
        CreatedAt = DateTime.UtcNow,
    };

    // ========== Route Mappings ==========
    private static RouteInfo MapToRouteInfo(MasterRoute e) => new()
    {
        RouteId = e.RouteId,
        RouteName = e.RouteName,
        RouteCode = e.RouteId,
        RouteVersion = e.RouteVersion,
        ProductId = e.ProductId,
        Version = e.RouteVersion,
        Status = e.IsActive ? "Active" : "Inactive",
        Description = e.PackageType ?? string.Empty,
        PackageType = e.PackageType ?? string.Empty,
        IsActive = e.IsActive,
        IsApproved = e.IsApproved,
        ApprovedBy = e.ApprovedBy ?? string.Empty,
        ApprovedAt = e.ApprovedAt,
        Steps = [],
    };

    private static MasterRoute MapToRouteEntity(RouteInfo m) => new()
    {
        RouteId = m.RouteId,
        RouteName = m.RouteName,
        RouteVersion = m.Version,
        ProductId = m.ProductId,
        PackageType = m.Description,
        IsActive = m.Status == "Active",
        CreatedAt = DateTime.UtcNow,
        Steps = [],
    };

    // ========== Material Mappings ==========
    private static MaterialInfo MapToMaterialInfo(MasterMaterial e) => new()
    {
        MaterialId = e.MaterialId,
        MaterialName = e.MaterialName,
        MaterialCode = e.MaterialId,
        MaterialType = e.MaterialType,
        Supplier = e.Supplier,
        Unit = e.Unit,
        MinStock = e.MinStock,
        Status = e.IsActive ? "Active" : "Inactive",
        Description = e.Specification ?? string.Empty,
    };

    private static MasterMaterial MapToMaterialEntity(MaterialInfo m) => new()
    {
        MaterialId = m.MaterialId,
        MaterialName = m.MaterialName,
        MaterialType = m.MaterialType,
        Specification = m.Description,
        Unit = m.Unit,
        Supplier = m.Supplier,
        MinStock = (int)m.MinStock,
        CurrentStock = 0,
        IsActive = m.Status == "Active",
        CreatedAt = DateTime.UtcNow,
    };

    // ========== YieldRule Mappings ==========
    private static YieldRule MapToYieldRule(MasterYieldRule e) => new()
    {
        RuleId = e.RuleId,
        RouteId = e.RouteId,
        StepCode = e.StepCode,
        WarningYield = (double)e.YieldThreshold - 5,
        AlarmYield = (double)e.YieldThreshold - 10,
        HoldYield = (double)e.YieldThreshold,
    };

    private static MasterYieldRule MapToYieldRuleEntity(YieldRule m) => new()
    {
        RuleId = m.RuleId,
        RouteId = m.RouteId,
        StepCode = m.StepCode,
        YieldThreshold = (decimal)m.HoldYield,
        ActionType = "AutoHold",
        NotifyRole = "QA",
        IsActive = true,
        CreatedAt = DateTime.UtcNow,
    };

    // ========== AlarmRule Mappings ==========
    private static AlarmRule MapToAlarmRule(MasterAlarmRule e) => new()
    {
        RuleId = e.RuleId,
        RuleName = e.AlarmType,
        AlarmType = e.AlarmType,
        Condition = e.Severity,
        NotifyRoles = e.NotifyRole ?? string.Empty,
        IsActive = e.IsEnabled,
    };

    private static MasterAlarmRule MapToAlarmRuleEntity(AlarmRule m) => new()
    {
        RuleId = m.RuleId,
        AlarmType = m.AlarmType,
        Severity = m.Condition ?? "Warning",
        IsEnabled = m.IsActive,
        NotifyRole = m.NotifyRoles,
        CreatedAt = DateTime.UtcNow,
    };

    // ========== ScrapRule Mappings ==========
    private static ScrapRule MapToScrapRule(MasterScrapRule e) => new()
    {
        RuleId = e.RuleId,
        RouteId = e.RouteId ?? string.Empty,
        StepCode = e.StepCode ?? string.Empty,
        MaxScrapQty = 0,
        MaxScrapPercent = (double)e.ThresholdPercent,
        ApprovalLevel = e.ApprovalLevel ?? string.Empty,
    };

    private static MasterScrapRule MapToScrapRuleEntity(ScrapRule m) => new()
    {
        RuleId = m.RuleId,
        RouteId = m.RouteId,
        StepCode = m.StepCode,
        ThresholdPercent = (decimal)m.MaxScrapPercent,
        RequiresApproval = true,
        ApprovalLevel = m.ApprovalLevel,
        IsActive = true,
        CreatedAt = DateTime.UtcNow,
    };

    // ========== Customer Mappings ==========
    private static CustomerInfo MapToCustomerInfo(MasterCustomer e) => new()
    {
        CustomerId = e.CustomerId,
        CustomerName = e.CustomerName,
        CustomerCode = e.CustomerCode,
        ContactPerson = e.ContactPerson,
        ContactPhone = e.ContactPhone,
        Email = e.Email,
        Address = e.Address,
        CustomerPnPrefix = e.CustomerPnPrefix,
        QualityLevel = e.QualityLevel,
        SpecialRequirements = e.SpecialRequirements,
        DefaultPackingSpec = e.DefaultPackingSpec,
        DefaultOqcSpec = e.DefaultOqcSpec,
        Status = e.Status,
    };

    private static MasterCustomer MapToCustomerEntity(CustomerInfo m) => new()
    {
        CustomerId = m.CustomerId,
        CustomerName = m.CustomerName,
        CustomerCode = m.CustomerCode,
        ContactPerson = m.ContactPerson,
        ContactPhone = m.ContactPhone,
        Email = m.Email,
        Address = m.Address,
        CustomerPnPrefix = m.CustomerPnPrefix,
        QualityLevel = m.QualityLevel,
        SpecialRequirements = m.SpecialRequirements,
        DefaultPackingSpec = m.DefaultPackingSpec,
        DefaultOqcSpec = m.DefaultOqcSpec,
        Status = m.Status,
        CreatedAt = DateTime.UtcNow,
    };

    // ========== ReasonCode Mappings ==========
    private static ReasonCodeInfo MapToReasonCodeInfo(MasterReasonCode e) => new()
    {
        ReasonCodeId = e.ReasonCodeId,
        Category = e.Category,
        SubCategory = e.SubCategory,
        ReasonText = e.ReasonText,
        ApplicableTo = e.ApplicableTo,
        IsEnabled = e.IsEnabled,
    };

    private static MasterReasonCode MapToReasonCodeEntity(ReasonCodeInfo m) => new()
    {
        ReasonCodeId = m.ReasonCodeId,
        Category = m.Category,
        SubCategory = m.SubCategory,
        ReasonText = m.ReasonText,
        ApplicableTo = m.ApplicableTo,
        IsEnabled = m.IsEnabled,
        CreatedAt = DateTime.UtcNow,
    };

    // ========== DefectCode Mappings ==========
    private static DefectCodeInfo MapToDefectCodeInfo(MasterDefectCode e) => new()
    {
        DefectCodeId = e.DefectCodeId,
        DefectCategory = e.DefectCategory,
        DefectText = e.DefectText,
        Severity = e.Severity,
        IsEnabled = e.IsEnabled,
    };

    private static MasterDefectCode MapToDefectCodeEntity(DefectCodeInfo m) => new()
    {
        DefectCodeId = m.DefectCodeId,
        DefectCategory = m.DefectCategory,
        DefectText = m.DefectText,
        Severity = m.Severity,
        IsEnabled = m.IsEnabled,
        CreatedAt = DateTime.UtcNow,
    };
}
