namespace WillTheyDie.Api.Models;

public class Episode
{
    public int Id { get; set; }
    public int SeasonId { get; set; }
    public int EpisodeNumber { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime? AirDate { get; set; }
    public bool IsBettingOpen { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public Season Season { get; set; } = null!;
    public ICollection<Bet> Bets { get; set; } = new List<Bet>();
}
