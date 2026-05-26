using MES.Modules.Production.Models;

namespace MES.Modules.Production.Services;

public class RecipeGateway : IRecipeGateway
{
    private readonly IMasterDataService _masterData;

    public RecipeGateway(IMasterDataService masterData)
    {
        _masterData = masterData;
    }

    public async Task<bool> IsRecipeApprovedAsync(string recipeId, string stepCode, string equipmentId)
    {
        if (string.IsNullOrEmpty(recipeId)) return true;

        var recipe = await _masterData.GetRecipeAsync(recipeId);
        if (recipe is null) return false;

        if (!recipe.IsActive) return false;

        if (!string.IsNullOrEmpty(recipe.StepCode) && recipe.StepCode != stepCode) return false;

        return await IsRecipeMatchEquipmentAsync(recipeId, equipmentId);
    }

    public async Task<bool> IsRecipeMatchEquipmentAsync(string recipeId, string equipmentId)
    {
        var recipe = await _masterData.GetRecipeAsync(recipeId);
        var equipment = await _masterData.GetEquipmentAsync(equipmentId);

        if (recipe is null || equipment is null) return false;

        if (string.IsNullOrEmpty(recipe.EquipmentGroup)) return true;
        return recipe.EquipmentGroup == equipment.EquipmentGroup;
    }

    public async Task<List<string>> GetAvailableRecipesAsync(string stepCode, string equipmentGroup)
    {
        var allRecipes = await _masterData.GetAllRecipesAsync();
        return allRecipes
            .Where(r => r.IsActive &&
                       (string.IsNullOrEmpty(r.StepCode) || r.StepCode == stepCode) &&
                       (string.IsNullOrEmpty(r.EquipmentGroup) || r.EquipmentGroup == equipmentGroup))
            .Select(r => r.RecipeId)
            .ToList();
    }
}
