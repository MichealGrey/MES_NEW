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

    public MasterDataService(
        IRepository<MasterEquipment> equipRepo,
        IRepository<MasterCarrier> carrierRepo,
        IRepository<MasterRecipe> recipeRepo,
        IRepository<SysUser> userRepo)
    {
        _equipRepo = equipRepo;
        _carrierRepo = carrierRepo;
        _recipeRepo = recipeRepo;
        _userRepo = userRepo;
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

    private static EquipmentInfo MapToEquipmentInfo(MasterEquipment e) => new()
    {
        EquipmentId = e.EquipmentId,
        EquipmentName = e.EquipmentName,
        EquipmentGroup = e.EquipmentGroup,
        EquipmentType = e.EquipmentType,
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
}
