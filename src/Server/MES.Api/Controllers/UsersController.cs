using Microsoft.AspNetCore.Mvc;
using MES.Contracts.Common;
using MES.Contracts.SystemMgmt;
using MES.Services.SystemMgmt;

namespace MES.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ApiResponse<PagedResult<UserResponse>>> GetUsers(
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null)
    {
        var result = await _userService.GetUsersAsync(pageIndex, pageSize, search);
        return ApiResponse<PagedResult<UserResponse>>.Ok(result);
    }

    [HttpGet("{userId}")]
    public async Task<ApiResponse<UserResponse?>> GetUser(string userId)
    {
        var result = await _userService.GetUserAsync(userId);
        return result != null
            ? ApiResponse<UserResponse?>.Ok(result)
            : ApiResponse<UserResponse?>.Fail("User not found");
    }

    [HttpPost]
    public async Task<ApiResponse<UserResponse>> CreateUser([FromBody] UserCreateRequest request)
    {
        var result = await _userService.CreateUserAsync(request);
        return ApiResponse<UserResponse>.Ok(result, "User created");
    }

    [HttpPut("{userId}")]
    public async Task<ApiResponse<bool>> UpdateUser(string userId, [FromBody] UserUpdateRequest request)
    {
        var result = await _userService.UpdateUserAsync(userId, request);
        return result
            ? ApiResponse<bool>.Ok(true, "User updated")
            : ApiResponse<bool>.Fail("User not found");
    }

    [HttpDelete("{userId}")]
    public async Task<ApiResponse<bool>> DeleteUser(string userId)
    {
        var result = await _userService.DeleteUserAsync(userId);
        return result
            ? ApiResponse<bool>.Ok(true, "User deleted")
            : ApiResponse<bool>.Fail("User not found");
    }

    [HttpPost("{userId}/reset-password")]
    public async Task<ApiResponse<bool>> ResetPassword(string userId, [FromBody] ResetPasswordRequest request)
    {
        var result = await _userService.ResetPasswordAsync(userId, request.NewPassword);
        return result
            ? ApiResponse<bool>.Ok(true, "Password reset successfully")
            : ApiResponse<bool>.Fail("User not found");
    }

    [HttpPost("{userId}/toggle-status")]
    public async Task<ApiResponse<bool>> ToggleUserStatus(string userId)
    {
        var result = await _userService.ToggleUserStatusAsync(userId);
        return result
            ? ApiResponse<bool>.Ok(true, "User status toggled")
            : ApiResponse<bool>.Fail("User not found");
    }
}
