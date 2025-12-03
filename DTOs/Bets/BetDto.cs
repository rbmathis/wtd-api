namespace WillTheyDie.Api.DTOs.Bets;

public class BetDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int CharacterId { get; set; }
    public string CharacterName { get; set; } = string.Empty;
    public int EpisodeId { get; set; }
    public string EpisodeTitle { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Prediction { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime PlacedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
}
