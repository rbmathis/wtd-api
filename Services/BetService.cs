using Microsoft.EntityFrameworkCore;
using WillTheyDie.Api.Data;
using WillTheyDie.Api.DTOs.Bets;
using WillTheyDie.Api.Models;

namespace WillTheyDie.Api.Services;

public class BetService : IBetService
{
    private readonly ApplicationDbContext _context;

    public BetService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<BetResultDto?> PlaceBetAsync(int userId, PlaceBetRequest request)
    {
        var episode = await _context.Episodes.FindAsync(request.EpisodeId);
        if (episode == null || !episode.IsBettingOpen)
        {
            return null;
        }

        var character = await _context.Characters.FindAsync(request.CharacterId);
        if (character == null || character.Status != "alive")
        {
            return null;
        }

        var season = await _context.Seasons
            .Include(s => s.Show)
            .FirstOrDefaultAsync(s => s.Id == episode.SeasonId);

        if (season == null)
        {
            return null;
        }

        var userShow = await _context.UserShows
            .FirstOrDefaultAsync(us => us.UserId == userId && us.ShowId == season.ShowId);

        if (userShow == null || userShow.CurrencyBalance < request.Amount)
        {
            return null;
        }

        if (request.Prediction != "dies" && request.Prediction != "survives")
        {
            return null;
        }

        var bet = new Bet
        {
            UserId = userId,
            CharacterId = request.CharacterId,
            EpisodeId = request.EpisodeId,
            Amount = request.Amount,
            Prediction = request.Prediction,
            Status = "pending",
            PlacedAt = DateTime.UtcNow
        };

        userShow.CurrencyBalance -= request.Amount;

        _context.Bets.Add(bet);
        await _context.SaveChangesAsync();

        return new BetResultDto
        {
            Success = true,
            BetId = bet.Id,
            NewBalance = userShow.CurrencyBalance
        };
    }

    public async Task<IEnumerable<BetDto>> GetUserBetsAsync(int userId, int? episodeId = null)
    {
        var query = _context.Bets
            .Include(b => b.Character)
            .Include(b => b.Episode)
            .Where(b => b.UserId == userId);

        if (episodeId.HasValue)
        {
            query = query.Where(b => b.EpisodeId == episodeId.Value);
        }

        return await query
            .Select(b => new BetDto
            {
                Id = b.Id,
                CharacterId = b.CharacterId,
                CharacterName = b.Character.Name,
                EpisodeId = b.EpisodeId,
                EpisodeTitle = b.Episode.Title,
                Amount = b.Amount,
                Prediction = b.Prediction,
                Status = b.Status,
                PlacedAt = b.PlacedAt,
                ResolvedAt = b.ResolvedAt
            })
            .OrderByDescending(b => b.PlacedAt)
            .ToListAsync();
    }

    public async Task<bool> ResolveBetsAsync(int episodeId, int characterId, bool died)
    {
        var bets = await _context.Bets
            .Include(b => b.User)
                .ThenInclude(u => u.UserShows)
            .Include(b => b.Episode)
                .ThenInclude(e => e.Season)
            .Where(b => b.EpisodeId == episodeId && b.CharacterId == characterId && b.Status == "pending")
            .ToListAsync();

        foreach (var bet in bets)
        {
            var wonBet = (died && bet.Prediction == "dies") || (!died && bet.Prediction == "survives");
            bet.Status = wonBet ? "won" : "lost";
            bet.ResolvedAt = DateTime.UtcNow;

            if (wonBet)
            {
                var userShow = bet.User.UserShows
                    .FirstOrDefault(us => us.ShowId == bet.Episode.Season.ShowId);

                if (userShow != null)
                {
                    userShow.CurrencyBalance += bet.Amount * 2;
                }
            }
        }

        await _context.SaveChangesAsync();
        return true;
    }
}
