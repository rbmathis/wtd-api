namespace WillTheyDie.Api.DTOs.Bets;

public class PlaceBetRequest
{
    public int CharacterId { get; set; }
    public int EpisodeId { get; set; }
    public decimal Amount { get; set; }
    public string Prediction { get; set; } = string.Empty; // "dies" or "survives"
}
