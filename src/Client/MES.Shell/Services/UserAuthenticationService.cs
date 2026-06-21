using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using MES.Shell.Models;

namespace MES.Shell.Services;

/// <summary>
/// 用户认证服务 — 通过后端 API 进行认证（主数据库 MySQL）
/// </summary>
public class UserAuthenticationService : IUserAuthenticationService
{
    private readonly HttpClient _httpClient;
    private static readonly System.Diagnostics.TraceSource _log =
        new System.Diagnostics.TraceSource("MES.Auth", System.Diagnostics.SourceLevels.All);

    // JSON 序列化选项：忽略大小写
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public UserAuthenticationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<UserInfo?> AuthenticateAsync(string employeeId, string? password)
    {
        if (string.IsNullOrWhiteSpace(employeeId))
            return null;

        try
        {
            var request = new { UserId = employeeId.Trim(), Password = password ?? string.Empty };
            _log.TraceEvent(System.Diagnostics.TraceEventType.Information, 0,
                $"[Login] Attempting login: UserId={request.UserId}, BaseAddress={_httpClient.BaseAddress}");

            var response = await _httpClient.PostAsJsonAsync("auth/login", request);
            var rawJson = await response.Content.ReadAsStringAsync();
            _log.TraceEvent(System.Diagnostics.TraceEventType.Information, 0,
                $"[Login] Response status: {(int)response.StatusCode} {response.StatusCode}");
            _log.TraceEvent(System.Diagnostics.TraceEventType.Information, 0,
                $"[Login] Raw JSON: {rawJson}");

            if (!response.IsSuccessStatusCode)
            {
                _log.TraceEvent(System.Diagnostics.TraceEventType.Warning, 0,
                    $"[Login] HTTP error: {response.StatusCode}");
                return null;
            }

            var content = JsonSerializer.Deserialize<LoginApiResponse>(rawJson, _jsonOptions);
            _log.TraceEvent(System.Diagnostics.TraceEventType.Information, 0,
                $"[Login] Deserialized: Success={content?.Success}, HasData={content?.Data != null}");

            if (content == null || !content.Success || content.Data == null)
            {
                _log.TraceEvent(System.Diagnostics.TraceEventType.Warning, 0,
                    $"[Login] Validation failed: content={content != null}, success={content?.Success}, dataNull={content?.Data == null}");
                return null;
            }

            _log.TraceEvent(System.Diagnostics.TraceEventType.Information, 0,
                $"[Login] SUCCESS: User={content.Data.UserName} ({content.Data.UserId})");

            return new UserInfo
            {
                EmployeeId = content.Data.UserId,
                DisplayName = content.Data.UserName,
                DepartmentId = content.Data.DeptId,
                RoleId = content.Data.RoleId,
                IsActive = true,
                LastLoginTime = DateTime.Now
            };
        }
        catch (HttpRequestException ex)
        {
            _log.TraceEvent(System.Diagnostics.TraceEventType.Error, 0,
                $"[Login] HttpRequestException: {ex.Message}");
            return null;
        }
        catch (Exception ex)
        {
            _log.TraceEvent(System.Diagnostics.TraceEventType.Error, 0,
                $"[Login] UNEXPECTED ERROR: {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
            return null;
        }
    }

    private class LoginApiResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("data")]
        public LoginData? Data { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }
    }

    private class LoginData
    {
        [JsonPropertyName("userId")]
        public string UserId { get; set; } = string.Empty;

        [JsonPropertyName("userName")]
        public string UserName { get; set; } = string.Empty;

        [JsonPropertyName("roleId")]
        public string RoleId { get; set; } = string.Empty;

        [JsonPropertyName("deptId")]
        public string DeptId { get; set; } = string.Empty;
    }
}
