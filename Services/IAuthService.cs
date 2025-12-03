using WillTheyDie.Api.DTOs.Auth;

namespace WillTheyDie.Api.Services;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
    Task<LoginResponse?> RegisterAsync(RegisterRequest request);
    Task<UserProfileDto?> GetUserProfileAsync(int userId);
}
