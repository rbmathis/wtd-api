namespace WillTheyDie.Api.Models;

public class Character
{
    public int Id { get; set; }
    public int ShowId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Actor { get; set; }
    public string? ImageUrl { get; set; }
    public string Status { get; set; } = "alive"; // alive, dead, unknown
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public Show Show { get; set; } = null!;
    public ICollection<Bet> Bets { get; set; } = new List<Bet>();
}
