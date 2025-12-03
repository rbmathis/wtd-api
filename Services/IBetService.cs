using WillTheyDie.Api.DTOs.Bets;

namespace WillTheyDie.Api.Services;

public interface IBetService
{
    Task<BetResultDto?> PlaceBetAsync(int userId, PlaceBetRequest request);
    Task<IEnumerable<BetDto>> GetUserBetsAsync(int userId, int? episodeId = null);
    Task<bool> ResolveBetsAsync(int episodeId, int characterId, bool died);
}
