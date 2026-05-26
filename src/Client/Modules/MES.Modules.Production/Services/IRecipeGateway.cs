namespace MES.Modules.Production.Services;

public interface IRecipeGateway
{
    Task<bool> IsRecipeApprovedAsync(string recipeId, string stepCode, string equipmentId);
    Task<bool> IsRecipeMatchEquipmentAsync(string recipeId, string equipmentId);
    Task<List<string>> GetAvailableRecipesAsync(string stepCode, string equipmentGroup);
}
