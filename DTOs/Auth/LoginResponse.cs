namespace WillTheyDie.Api.DTOs.Auth;

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public UserProfileDto User { get; set; } = null!;
}
