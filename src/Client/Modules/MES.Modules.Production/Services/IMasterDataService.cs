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

    // Product
    Task<List<ProductInfo>> GetAllProductsAsync();
    Task<ProductInfo?> GetProductAsync(string productId);
    Task SaveProductAsync(ProductInfo product);
    Task DeleteProductAsync(string productId);

    // Route
    Task<List<RouteInfo>> GetAllRoutesAsync();
    Task<RouteInfo?> GetRouteAsync(string routeId);
    Task SaveRouteAsync(RouteInfo route);
    Task DeleteRouteAsync(string routeId);

    // Material
    Task<List<MaterialInfo>> GetAllMaterialsAsync();
    Task<MaterialInfo?> GetMaterialAsync(string materialId);
    Task SaveMaterialAsync(MaterialInfo material);
    Task DeleteMaterialAsync(string materialId);

    // Yield Rule
    Task<List<YieldRule>> GetAllYieldRulesAsync();
    Task SaveYieldRuleAsync(YieldRule rule);
    Task DeleteYieldRuleAsync(string ruleId);

    // Alarm Rule
    Task<List<AlarmRule>> GetAllAlarmRulesAsync();
    Task SaveAlarmRuleAsync(AlarmRule rule);
    Task DeleteAlarmRuleAsync(string ruleId);

    // Scrap Rule
    Task<List<ScrapRule>> GetAllScrapRulesAsync();
    Task SaveScrapRuleAsync(ScrapRule rule);
    Task DeleteScrapRuleAsync(string ruleId);

    // Customer
    Task<List<CustomerInfo>> GetAllCustomersAsync();
    Task<CustomerInfo?> GetCustomerAsync(string customerId);
    Task SaveCustomerAsync(CustomerInfo customer);
    Task UpdateCustomerStatusAsync(string customerId, string status);

    // Reason Code
    Task<List<ReasonCodeInfo>> GetAllReasonCodesAsync();
    Task SaveReasonCodeAsync(ReasonCodeInfo code);
    Task DeleteReasonCodeAsync(string codeId);

    // Defect Code
    Task<List<DefectCodeInfo>> GetAllDefectCodesAsync();
    Task SaveDefectCodeAsync(DefectCodeInfo code);
    Task DeleteDefectCodeAsync(string codeId);

    // Query helpers
    Task<List<EquipmentInfo>> GetAvailableEquipmentsAsync(string equipmentGroup);
    Task<List<RecipeInfo>> GetRecipesForStepAsync(string equipmentGroup, string stepCode);
}
