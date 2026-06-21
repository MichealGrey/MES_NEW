using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MES.Infrastructure.Persistence.Repositories;

namespace MES.Infrastructure.Persistence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMesPersistence(this IServiceCollection services, string connectionString)
    {
        // Register DbContext
        services.AddDbContext<MesDbContext>(options =>
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString),
                mySqlOptions => mySqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorNumbersToAdd: null)));

        // Register Repositories
        services.AddScoped<IRouteRepository, RouteRepository>();
        services.AddScoped<ILotRepository, LotRepository>();
        services.AddScoped<IWorkOrderRepository, WorkOrderRepository>();
        services.AddScoped<IEquipmentRepository, EquipmentRepository>();
        services.AddScoped<ICarrierRepository, CarrierRepository>();
        services.AddScoped<IRecipeRepository, RecipeRepository>();
        services.AddScoped<IYieldRuleRepository, YieldRuleRepository>();
        services.AddScoped<IAlarmRuleRepository, AlarmRuleRepository>();
        services.AddScoped<IScrapRuleRepository, ScrapRuleRepository>();
        services.AddScoped<IOperationHistoryRepository, OperationHistoryRepository>();
        services.AddScoped<IScrapRecordRepository, ScrapRecordRepository>();
        services.AddScoped<IReworkRecordRepository, ReworkRecordRepository>();
        services.AddScoped<ILotSplitRepository, LotSplitRepository>();
        services.AddScoped<ILotMergeRepository, LotMergeRepository>();
        services.AddScoped<ICarrierBindingRepository, CarrierBindingRepository>();
        services.AddScoped<ISignatureRepository, SignatureRepository>();
        services.AddScoped<IAuditTrailRepository, AuditTrailRepository>();
        services.AddScoped<IAlarmRepository, AlarmRepository>();
        services.AddScoped<IDispatchTaskRepository, DispatchTaskRepository>();
        services.AddScoped<IQuantityTransactionRepository, QuantityTransactionRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        services.AddScoped<ISignatureLevelRepository, SignatureLevelRepository>();
        services.AddScoped<IPermissionConfirmRepository, PermissionConfirmRepository>();

        // Register generic repository for all entities
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        return services;
    }
}
