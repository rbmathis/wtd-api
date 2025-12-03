using WillTheyDie.Api.DTOs.Shows;
using WillTheyDie.Api.DTOs.Characters;
using WillTheyDie.Api.DTOs.Leaderboard;
using WillTheyDie.Api.DTOs.Episodes;

namespace WillTheyDie.Api.Services;

public interface IShowService
{
    Task<IEnumerable<ShowDto>> GetAllShowsAsync();
    Task<ShowDetailDto?> GetShowDetailAsync(int showId);
    Task<bool> JoinShowAsync(int userId, int showId);
    Task<decimal?> GetUserBalanceAsync(int userId, int showId);
    Task<IEnumerable<CharacterDto>> GetShowCharactersAsync(int showId, bool? aliveOnly = null);
    Task<IEnumerable<LeaderboardEntryDto>> GetLeaderboardAsync(int showId, int limit = 10);
    Task<IEnumerable<EpisodeDto>> GetSeasonEpisodesAsync(int seasonId);
    Task<EpisodeDto?> GetEpisodeDetailAsync(int episodeId);
}
