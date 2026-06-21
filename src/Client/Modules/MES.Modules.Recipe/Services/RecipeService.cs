using MES.Modules.Recipe.Models;

namespace MES.Modules.Recipe.Services;

/// <summary>
/// Recipe service interface for managing recipes, parameters, equipment bindings, approvals, dispatch, and versions.
/// </summary>
public interface IRecipeService
{
    Task<List<RecipeListModel>> GetAllRecipesAsync();
    Task<List<RecipeParameterItem>> GetRecipeParametersAsync(string recipeId);
    Task<List<RecipeEquipmentItem>> GetRecipeEquipmentBindingsAsync();
    Task<List<RecipeApprovalItem>> GetPendingApprovalsAsync();
    Task<List<RecipeListModel>> GetDispatchableRecipesAsync();
    Task<List<RecipeListModel>> GetRecipeVersionsAsync(string recipeId);

    Task<bool> SaveParameterAsync(RecipeParameterItem parameter);
    Task<bool> DeleteParameterAsync(string parameterId);
    Task<bool> BindRecipeToEquipmentAsync(string recipeId, string equipmentId);
    Task<bool> UnbindRecipeFromEquipmentAsync(string bindingId);
    Task<bool> ApproveRecipeAsync(string approvalId, string comment);
    Task<bool> RejectRecipeAsync(string approvalId, string reason);
    Task<bool> DispatchRecipeAsync(string recipeId, string equipmentId);
    Task<bool> CreateNewVersionAsync(string recipeId, string newVersion);

    Task<RecipeStatistics> GetStatisticsAsync();
}

public class RecipeStatistics
{
    public int TotalRecipes { get; set; }
    public int ActiveRecipes { get; set; }
    public int DraftRecipes { get; set; }
    public int ObsoleteRecipes { get; set; }
    public int PendingApprovals { get; set; }
    public int BoundEquipment { get; set; }
    public int DispatchedCount { get; set; }
    public int TotalVersions { get; set; }
}

public class InMemoryRecipeService : IRecipeService
{
    private readonly List<RecipeListModel> _recipes = new();
    private readonly List<RecipeParameterItem> _parameters = new();
    private readonly List<RecipeEquipmentItem> _equipmentBindings = new();
    private readonly List<RecipeApprovalItem> _approvals = new();

    public InMemoryRecipeService()
    {
        SeedData();
    }

    private void SeedData()
    {
        _recipes.AddRange(new[]
        {
            new RecipeListModel { Id = "RCP-001", RecipeId = "RCP-WB-001", RecipeName = "WB Standard Recipe", EquipmentType = "Wire Bonder", Version = "1.0", Status = "Active", CreatedBy = "张工", CreatedDate = DateTime.Now.AddDays(-30) },
            new RecipeListModel { Id = "RCP-002", RecipeId = "RCP-DB-001", RecipeName = "DB Standard Recipe", EquipmentType = "Die Bonder", Version = "2.0", Status = "Active", CreatedBy = "李工", CreatedDate = DateTime.Now.AddDays(-20) },
            new RecipeListModel { Id = "RCP-003", RecipeId = "RCP-MP-001", RecipeName = "MP Standard Recipe", EquipmentType = "Mold Press", Version = "1.5", Status = "Draft", CreatedBy = "王工", CreatedDate = DateTime.Now.AddDays(-10) },
            new RecipeListModel { Id = "RCP-004", RecipeId = "RCP-WS-001", RecipeName = "WS Standard Recipe", EquipmentType = "Wafer Saw", Version = "1.0", Status = "Active", CreatedBy = "赵工", CreatedDate = DateTime.Now.AddDays(-15) },
            new RecipeListModel { Id = "RCP-005", RecipeId = "RCP-TH-001", RecipeName = "TH Standard Recipe", EquipmentType = "Trim & Form", Version = "1.2", Status = "Obsolete", CreatedBy = "张工", CreatedDate = DateTime.Now.AddDays(-60) },
        });

        _parameters.AddRange(new[]
        {
            new RecipeParameterItem { Id = "RP-001", RecipeId = "RCP-WB-001", ParameterName = "BondForce", MinValue = 10, MaxValue = 50, TargetValue = 30, Unit = "gf" },
            new RecipeParameterItem { Id = "RP-002", RecipeId = "RCP-WB-001", ParameterName = "BondTime", MinValue = 10, MaxValue = 100, TargetValue = 50, Unit = "ms" },
            new RecipeParameterItem { Id = "RP-003", RecipeId = "RCP-DB-001", ParameterName = "Temperature", MinValue = 150, MaxValue = 250, TargetValue = 200, Unit = "°C" },
            new RecipeParameterItem { Id = "RP-004", RecipeId = "RCP-DB-001", ParameterName = "Power", MinValue = 5, MaxValue = 30, TargetValue = 15, Unit = "W" },
            new RecipeParameterItem { Id = "RP-005", RecipeId = "RCP-MP-001", ParameterName = "Pressure", MinValue = 100, MaxValue = 500, TargetValue = 300, Unit = "psi" },
        });

        _equipmentBindings.AddRange(new[]
        {
            new RecipeEquipmentItem { Id = "RE-001", EquipmentId = "WB-01", RecipeId = "RCP-WB-001", Status = "Bound", LastUsed = DateTime.Now.AddHours(-2) },
            new RecipeEquipmentItem { Id = "RE-002", EquipmentId = "WB-02", RecipeId = "RCP-WB-001", Status = "Bound", LastUsed = DateTime.Now.AddHours(-5) },
            new RecipeEquipmentItem { Id = "RE-003", EquipmentId = "DB-01", RecipeId = "RCP-DB-001", Status = "Bound", LastUsed = DateTime.Now.AddHours(-1) },
            new RecipeEquipmentItem { Id = "RE-004", EquipmentId = "MP-01", RecipeId = "RCP-MP-001", Status = "Bound", LastUsed = DateTime.Now.AddHours(-3) },
        });

        _approvals.AddRange(new[]
        {
            new RecipeApprovalItem { Id = "RA-001", RecipeId = "RCP-MP-001", RecipeName = "MP Standard Recipe", SubmittedBy = "王工", SubmittedDate = DateTime.Now.AddDays(-2), Status = "Pending", Reviewer = "张经理" },
            new RecipeApprovalItem { Id = "RA-002", RecipeId = "RCP-WB-002", RecipeName = "WB Advanced Recipe", SubmittedBy = "李工", SubmittedDate = DateTime.Now.AddDays(-5), Status = "Approved", Reviewer = "张经理", ApprovedDate = DateTime.Now.AddDays(-3) },
            new RecipeApprovalItem { Id = "RA-003", RecipeId = "RCP-DB-002", RecipeName = "DB High-Speed Recipe", SubmittedBy = "赵工", SubmittedDate = DateTime.Now.AddDays(-1), Status = "Rejected", Reviewer = "张经理", RejectionReason = "参数超出安全范围" },
        });
    }

    public Task<List<RecipeListModel>> GetAllRecipesAsync()
        => Task.FromResult(_recipes.ToList());

    public Task<List<RecipeParameterItem>> GetRecipeParametersAsync(string recipeId)
        => Task.FromResult(string.IsNullOrEmpty(recipeId)
            ? _parameters.ToList()
            : _parameters.Where(p => p.RecipeId == recipeId).ToList());

    public Task<List<RecipeEquipmentItem>> GetRecipeEquipmentBindingsAsync()
        => Task.FromResult(_equipmentBindings.ToList());

    public Task<List<RecipeApprovalItem>> GetPendingApprovalsAsync()
        => Task.FromResult(_approvals.ToList());

    public Task<List<RecipeListModel>> GetDispatchableRecipesAsync()
        => Task.FromResult(_recipes.Where(r => r.Status == "Active").ToList());

    public Task<List<RecipeListModel>> GetRecipeVersionsAsync(string recipeId)
        => Task.FromResult(string.IsNullOrEmpty(recipeId)
            ? _recipes.ToList()
            : _recipes.Where(r => r.RecipeId == recipeId).ToList());

    public Task<bool> SaveParameterAsync(RecipeParameterItem parameter)
    {
        var existing = _parameters.FirstOrDefault(p => p.Id == parameter.Id);
        if (existing != null)
        {
            existing.ParameterName = parameter.ParameterName;
            existing.MinValue = parameter.MinValue;
            existing.MaxValue = parameter.MaxValue;
            existing.TargetValue = parameter.TargetValue;
            existing.Unit = parameter.Unit;
        }
        else
        {
            parameter.Id = $"RP-{_parameters.Count + 1:D3}";
            _parameters.Add(parameter);
        }
        return Task.FromResult(true);
    }

    public Task<bool> DeleteParameterAsync(string parameterId)
    {
        var item = _parameters.FirstOrDefault(p => p.Id == parameterId);
        if (item != null)
        {
            _parameters.Remove(item);
        }
        return Task.FromResult(true);
    }

    public Task<bool> BindRecipeToEquipmentAsync(string recipeId, string equipmentId)
    {
        var binding = new RecipeEquipmentItem
        {
            Id = $"RE-{_equipmentBindings.Count + 1:D3}",
            RecipeId = recipeId,
            EquipmentId = equipmentId,
            Status = "Bound",
            LastUsed = DateTime.Now
        };
        _equipmentBindings.Add(binding);
        return Task.FromResult(true);
    }

    public Task<bool> UnbindRecipeFromEquipmentAsync(string bindingId)
    {
        var item = _equipmentBindings.FirstOrDefault(b => b.Id == bindingId);
        if (item != null)
        {
            _equipmentBindings.Remove(item);
        }
        return Task.FromResult(true);
    }

    public Task<bool> ApproveRecipeAsync(string approvalId, string comment)
    {
        var item = _approvals.FirstOrDefault(a => a.Id == approvalId);
        if (item != null && item.Status == "Pending")
        {
            item.Status = "Approved";
            item.ApprovedDate = DateTime.Now;
            item.Comment = comment;
        }
        return Task.FromResult(true);
    }

    public Task<bool> RejectRecipeAsync(string approvalId, string reason)
    {
        var item = _approvals.FirstOrDefault(a => a.Id == approvalId);
        if (item != null && item.Status == "Pending")
        {
            item.Status = "Rejected";
            item.RejectedDate = DateTime.Now;
            item.RejectionReason = reason;
        }
        return Task.FromResult(true);
    }

    public Task<bool> DispatchRecipeAsync(string recipeId, string equipmentId)
    {
        var recipe = _recipes.FirstOrDefault(r => r.Id == recipeId || r.RecipeId == recipeId);
        if (recipe != null)
        {
            recipe.CurrentEquipment = equipmentId;
            recipe.Status = "Dispatched";
        }
        return Task.FromResult(true);
    }

    public Task<bool> CreateNewVersionAsync(string recipeId, string newVersion)
    {
        var source = _recipes.FirstOrDefault(r => r.Id == recipeId || r.RecipeId == recipeId);
        if (source != null)
        {
            var newRecipe = source.Clone();
            newRecipe.Id = $"RCP-{_recipes.Count + 1:D3}";
            newRecipe.Version = newVersion;
            newRecipe.Status = "Draft";
            newRecipe.CreatedDate = DateTime.Now;
            _recipes.Add(newRecipe);
        }
        return Task.FromResult(true);
    }

    public Task<RecipeStatistics> GetStatisticsAsync()
    {
        return Task.FromResult(new RecipeStatistics
        {
            TotalRecipes = _recipes.Count,
            ActiveRecipes = _recipes.Count(r => r.Status == "Active"),
            DraftRecipes = _recipes.Count(r => r.Status == "Draft"),
            ObsoleteRecipes = _recipes.Count(r => r.Status == "Obsolete"),
            PendingApprovals = _approvals.Count(a => a.Status == "Pending"),
            BoundEquipment = _equipmentBindings.Count(e => e.Status == "Bound"),
            DispatchedCount = _recipes.Count(r => r.Status == "Dispatched"),
            TotalVersions = _recipes.Count
        });
    }
}
