using MES.Contracts.Common;
using MES.Contracts.SystemMgmt;

namespace MES.Services.SystemMgmt;

public interface IUserService
{
    Task<PagedResult<UserResponse>> GetUsersAsync(int pageIndex, int pageSize, string? search = null);
    Task<UserResponse?> GetUserAsync(string userId);
    Task<UserResponse> CreateUserAsync(UserCreateRequest request);
    Task<bool> UpdateUserAsync(string userId, UserUpdateRequest request);
    Task<bool> DeleteUserAsync(string userId);
    Task<bool> ResetPasswordAsync(string userId, string newPassword);
    Task<bool> ToggleUserStatusAsync(string userId);
}
