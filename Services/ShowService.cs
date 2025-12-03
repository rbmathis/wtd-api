using Microsoft.EntityFrameworkCore;
using WillTheyDie.Api.Data;
using WillTheyDie.Api.DTOs.Shows;
using WillTheyDie.Api.DTOs.Characters;
using WillTheyDie.Api.DTOs.Leaderboard;
using WillTheyDie.Api.DTOs.Episodes;
using WillTheyDie.Api.Models;

namespace WillTheyDie.Api.Services;

public class ShowService : IShowService
{
    private readonly ApplicationDbContext _context;

    public ShowService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ShowDto>> GetAllShowsAsync()
    {
        return await _context.Shows
            .Where(s => s.IsActive)
            .Select(s => new ShowDto
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                ImageUrl = s.ImageUrl,
                CurrencyName = s.CurrencyName,
                CurrencySymbol = s.CurrencySymbol,
                IsActive = s.IsActive
            })
            .ToListAsync();
    }

    public async Task<ShowDetailDto?> GetShowDetailAsync(int showId)
    {
        var show = await _context.Shows
            .Include(s => s.Seasons)
                .ThenInclude(season => season.Episodes)
            .Include(s => s.Characters)
            .FirstOrDefaultAsync(s => s.Id == showId);

        if (show == null)
        {
            return null;
        }

        return new ShowDetailDto
        {
            Id = show.Id,
            Name = show.Name,
            Description = show.Description,
            ImageUrl = show.ImageUrl,
            CurrencyName = show.CurrencyName,
            CurrencySymbol = show.CurrencySymbol,
            IsActive = show.IsActive,
            InitialBalance = show.InitialBalance,
            Seasons = show.Seasons.Select(s => new DTOs.Shows.SeasonDto
            {
                Id = s.Id,
                ShowId = s.ShowId,
                SeasonNumber = s.SeasonNumber,
                Name = s.Name,
                Episodes = s.Episodes.Select(e => new DTOs.Episodes.EpisodeDto
                {
                    Id = e.Id,
                    SeasonId = e.SeasonId,
                    EpisodeNumber = e.EpisodeNumber,
                    Title = e.Title,
                    AirDate = e.AirDate,
                    IsBettingOpen = e.IsBettingOpen
                }).ToList()
            }).ToList(),
            Characters = show.Characters.Select(c => new CharacterDto
            {
                Id = c.Id,
                ShowId = c.ShowId,
                Name = c.Name,
                Actor = c.Actor,
                ImageUrl = c.ImageUrl,
                Status = c.Status
            }).ToList()
        };
    }

    public async Task<bool> JoinShowAsync(int userId, int showId)
    {
        var show = await _context.Shows.FindAsync(showId);
        if (show == null)
        {
            return false;
        }

        var existingUserShow = await _context.UserShows
            .FirstOrDefaultAsync(us => us.UserId == userId && us.ShowId == showId);

        if (existingUserShow != null)
        {
            return false;
        }

        var userShow = new UserShow
        {
            UserId = userId,
            ShowId = showId,
            CurrencyBalance = show.InitialBalance,
            JoinedAt = DateTime.UtcNow
        };

        _context.UserShows.Add(userShow);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<decimal?> GetUserBalanceAsync(int userId, int showId)
    {
        var userShow = await _context.UserShows
            .FirstOrDefaultAsync(us => us.UserId == userId && us.ShowId == showId);

        return userShow?.CurrencyBalance;
    }

    public async Task<IEnumerable<CharacterDto>> GetShowCharactersAsync(int showId, bool? aliveOnly = null)
    {
        var query = _context.Characters.Where(c => c.ShowId == showId);

        if (aliveOnly == true)
        {
            query = query.Where(c => c.Status == "alive");
        }

        return await query
            .Select(c => new CharacterDto
            {
                Id = c.Id,
                ShowId = c.ShowId,
                Name = c.Name,
                Actor = c.Actor,
                ImageUrl = c.ImageUrl,
                Status = c.Status
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<LeaderboardEntryDto>> GetLeaderboardAsync(int showId, int limit = 10)
    {
        return await _context.UserShows
            .Where(us => us.ShowId == showId)
            .OrderByDescending(us => us.CurrencyBalance)
            .Take(limit)
            .Select(us => new LeaderboardEntryDto
            {
                UserId = us.UserId,
                Username = us.User.Username,
                Balance = us.CurrencyBalance,
                Rank = 0
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<EpisodeDto>> GetSeasonEpisodesAsync(int seasonId)
    {
        return await _context.Episodes
            .Where(e => e.SeasonId == seasonId)
            .OrderBy(e => e.EpisodeNumber)
            .Select(e => new EpisodeDto
            {
                Id = e.Id,
                SeasonId = e.SeasonId,
                EpisodeNumber = e.EpisodeNumber,
                Title = e.Title,
                AirDate = e.AirDate,
                IsBettingOpen = e.IsBettingOpen
            })
            .ToListAsync();
    }

    public async Task<EpisodeDto?> GetEpisodeDetailAsync(int episodeId)
    {
        var episode = await _context.Episodes
            .FirstOrDefaultAsync(e => e.Id == episodeId);

        if (episode == null)
        {
            return null;
        }

        return new EpisodeDto
        {
            Id = episode.Id,
            SeasonId = episode.SeasonId,
            EpisodeNumber = episode.EpisodeNumber,
            Title = episode.Title,
            AirDate = episode.AirDate,
            IsBettingOpen = episode.IsBettingOpen
        };
    }
}
