namespace WillTheyDie.Api.DTOs.Leaderboard;

public class LeaderboardEntryDto
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public int Rank { get; set; }
}
