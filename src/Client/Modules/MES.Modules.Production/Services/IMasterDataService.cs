using MES.Modules.Production.Models;

namespace MES.Modules.Production.Services;

public interface IMasterDataService
{
    // Equipment
    Task<List<EquipmentInfo>> GetAllEquipmentsAsync();
    Task<EquipmentInfo?> GetEquipmentAsync(string equipmentId);
    Task SaveEquipmentAsync(EquipmentInfo equipment);
    Task UpdateEquipmentStatusAsync(string equipmentId, string status);

    // Carrier
    Task<List<CarrierInfo>> GetAllCarriersAsync();
    Task<CarrierInfo?> GetCarrierAsync(string carrierId);
    Task SaveCarrierAsync(CarrierInfo carrier);
    Task UpdateCarrierStatusAsync(string carrierId, string status);

    // Recipe
    Task<List<RecipeInfo>> GetAllRecipesAsync();
    Task<RecipeInfo?> GetRecipeAsync(string recipeId);
    Task SaveRecipeAsync(RecipeInfo recipe);

    // User
    Task<List<UserInfo>> GetAllUsersAsync();
    Task<UserInfo?> GetUserAsync(string userId);
    Task SaveUserAsync(UserInfo user);

    // Query helpers
    Task<List<EquipmentInfo>> GetAvailableEquipmentsAsync(string equipmentGroup);
    Task<List<RecipeInfo>> GetRecipesForStepAsync(string equipmentGroup, string stepCode);
}
