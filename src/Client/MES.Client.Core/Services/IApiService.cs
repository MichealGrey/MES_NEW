namespace MES.Client.Core.Services;

public interface IApiService
{
    Task<T?> GetAsync<T>(string endpoint);
    Task<T?> PostAsync<T>(string endpoint, object data);
}
