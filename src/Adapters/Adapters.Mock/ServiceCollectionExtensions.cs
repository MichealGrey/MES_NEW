using MES.Adapters.Abstractions.CustomerPortal;
using MES.Adapters.Abstractions.Eap;
using MES.Adapters.Abstractions.Erp;
using MES.Adapters.Abstractions.Oa;
using MES.Adapters.Abstractions.Qms;
using MES.Adapters.Abstractions.Wms;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MES.Adapters.Mock;

/// <summary>
/// Mock 适配器服务注册扩展
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 注册所有 Mock 适配器
    /// </summary>
    public static IServiceCollection AddMockAdapters(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MockConfig>("MockErp", configuration.GetSection("Adapters:MockErp"));
        services.Configure<MockConfig>("MockEap", configuration.GetSection("Adapters:MockEap"));
        services.Configure<MockConfig>("MockWms", configuration.GetSection("Adapters:MockWms"));
        services.Configure<MockConfig>("MockQms", configuration.GetSection("Adapters:MockQms"));
        services.Configure<MockConfig>("MockOa", configuration.GetSection("Adapters:MockOa"));
        services.Configure<MockConfig>("MockCustomerPortal", configuration.GetSection("Adapters:MockCustomerPortal"));

        services.AddScoped<IMesErpAdapter, MockErpAdapter>();
        services.AddScoped<IMesEapAdapter, MockEapAdapter>();
        services.AddScoped<IMesWmsAdapter, MockWmsAdapter>();
        services.AddScoped<IMesQmsAdapter, MockQmsAdapter>();
        services.AddScoped<IMesOaAdapter, MockOaAdapter>();
        services.AddScoped<IMesCustomerPortalAdapter, MockCustomerPortalAdapter>();

        return services;
    }
}
