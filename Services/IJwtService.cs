using WillTheyDie.Api.Models;

namespace WillTheyDie.Api.Services;

public interface IJwtService
{
    string GenerateToken(User user);
    int? ValidateToken(string token);
}
