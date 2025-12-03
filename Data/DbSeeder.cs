using WillTheyDie.Api.Models;

namespace WillTheyDie.Api.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Check if database already has data
        if (context.Shows.Any())
        {
            return;
        }

        // Create shows
        var gameOfThrones = new Show
        {
            Name = "Game of Thrones",
            Description = "Nine noble families fight for control over the lands of Westeros, while an ancient enemy returns after being dormant for millennia.",
            ImageUrl = "https://image.tmdb.org/t/p/w500/1XS1oqL89opfnbLl8WnZY1O1uJx.jpg",
            CurrencyName = "Dragons",
            CurrencySymbol = "🐉",
            IsActive = true
        };

        var walkingDead = new Show
        {
            Name = "The Walking Dead",
            Description = "Sheriff Deputy Rick Grimes wakes up from a coma to learn the world is in ruins and must lead a group of survivors to stay alive.",
            ImageUrl = "https://image.tmdb.org/t/p/w500/xf9wuDcqlUPWABZNeDKPbZUjWx0.jpg",
            CurrencyName = "Bullets",
            CurrencySymbol = "🔫",
            IsActive = true
        };

        var breakingBad = new Show
        {
            Name = "Breaking Bad",
            Description = "A high school chemistry teacher diagnosed with cancer turns to producing and selling methamphetamine in order to secure his family's future.",
            ImageUrl = "https://image.tmdb.org/t/p/w500/3xnWaLQjelJDDF7LT1WBo6f4BRe.jpg",
            CurrencyName = "Blue Crystals",
            CurrencySymbol = "💎",
            IsActive = true
        };

        context.Shows.AddRange(gameOfThrones, walkingDead, breakingBad);
        await context.SaveChangesAsync();

        // Create Season 1 for Game of Thrones
        var gotSeason1 = new Season
        {
            ShowId = gameOfThrones.Id,
            SeasonNumber = 1,
            Name = "Season 1",
            IsActive = true
        };

        context.Seasons.Add(gotSeason1);
        await context.SaveChangesAsync();

        // Create episodes for GoT Season 1
        var episodes = new List<Episode>
        {
            new Episode { SeasonId = gotSeason1.Id, EpisodeNumber = 1, Title = "Winter Is Coming", IsBettingOpen = true, AirDate = new DateTime(2011, 4, 17) },
            new Episode { SeasonId = gotSeason1.Id, EpisodeNumber = 2, Title = "The Kingsroad", IsBettingOpen = false, AirDate = new DateTime(2011, 4, 24) },
            new Episode { SeasonId = gotSeason1.Id, EpisodeNumber = 3, Title = "Lord Snow", IsBettingOpen = false, AirDate = new DateTime(2011, 5, 1) },
            new Episode { SeasonId = gotSeason1.Id, EpisodeNumber = 4, Title = "Cripples, Bastards, and Broken Things", IsBettingOpen = false, AirDate = new DateTime(2011, 5, 8) },
            new Episode { SeasonId = gotSeason1.Id, EpisodeNumber = 5, Title = "The Wolf and the Lion", IsBettingOpen = false, AirDate = new DateTime(2011, 5, 15) },
        };

        context.Episodes.AddRange(episodes);
        await context.SaveChangesAsync();

        // Create characters for Game of Thrones
        var characters = new List<Character>
        {
            new Character { ShowId = gameOfThrones.Id, Name = "Jon Snow", Actor = "Kit Harington", ImageUrl = "https://image.tmdb.org/t/p/w185/gBiB5FJEz5lBIz9p7hT3K8M87fG.jpg", Status = "alive", IsActive = true },
            new Character { ShowId = gameOfThrones.Id, Name = "Daenerys Targaryen", Actor = "Emilia Clarke", ImageUrl = "https://image.tmdb.org/t/p/w185/j7d083zIMhwnKth8Xkdtw0ZHJXB.jpg", Status = "alive", IsActive = true },
            new Character { ShowId = gameOfThrones.Id, Name = "Tyrion Lannister", Actor = "Peter Dinklage", ImageUrl = "https://image.tmdb.org/t/p/w185/kJSN7ufKWLWeE0iyT0QxzF0r7Cd.jpg", Status = "alive", IsActive = true },
            new Character { ShowId = gameOfThrones.Id, Name = "Arya Stark", Actor = "Maisie Williams", ImageUrl = "https://image.tmdb.org/t/p/w185/sARC8IBoJhzqy0u5ZdYCVaMN2X.jpg", Status = "alive", IsActive = true },
            new Character { ShowId = gameOfThrones.Id, Name = "Ned Stark", Actor = "Sean Bean", ImageUrl = "https://image.tmdb.org/t/p/w185/8GoENbnl5hZnJhiP3wZF9rRm3T3.jpg", Status = "dead", IsActive = false },
            new Character { ShowId = gameOfThrones.Id, Name = "Cersei Lannister", Actor = "Lena Headey", ImageUrl = "https://image.tmdb.org/t/p/w185/8GoENbnl5hZnJhiP3wZF9rRm3T3.jpg", Status = "alive", IsActive = true },
        };

        context.Characters.AddRange(characters);
        await context.SaveChangesAsync();

        // Create test users with hashed passwords
        var testUser1 = new User
        {
            Username = "testuser1",
            Email = "testuser1@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
            CreatedAt = DateTime.UtcNow
        };

        var testUser2 = new User
        {
            Username = "testuser2",
            Email = "testuser2@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
            CreatedAt = DateTime.UtcNow
        };

        context.Users.AddRange(testUser1, testUser2);
        await context.SaveChangesAsync();

        // Create UserShow relationships (users joined shows)
        var userShows = new List<UserShow>
        {
            new UserShow { UserId = testUser1.Id, ShowId = gameOfThrones.Id, CurrencyBalance = 5000m, JoinedAt = DateTime.UtcNow },
            new UserShow { UserId = testUser2.Id, ShowId = gameOfThrones.Id, CurrencyBalance = 4500m, JoinedAt = DateTime.UtcNow },
        };

        context.UserShows.AddRange(userShows);
        await context.SaveChangesAsync();
    }
}
