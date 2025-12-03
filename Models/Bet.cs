namespace WillTheyDie.Api.Models;

public class Bet
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int CharacterId { get; set; }
    public int EpisodeId { get; set; }
    public decimal Amount { get; set; }
    public string Prediction { get; set; } = string.Empty; // "dies" or "survives"
    public string Status { get; set; } = "pending"; // "pending", "won", "lost", "refunded"
    public DateTime PlacedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ResolvedAt { get; set; }
    
    // Navigation properties
    public User User { get; set; } = null!;
    public Character Character { get; set; } = null!;
    public Episode Episode { get; set; } = null!;
}
