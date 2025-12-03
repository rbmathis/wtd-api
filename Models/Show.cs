namespace WillTheyDie.Api.Models;

public class Show
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string CurrencyName { get; set; } = "Coins";
    public string CurrencySymbol { get; set; } = "🪙";
    public decimal InitialBalance { get; set; } = 1000m;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public ICollection<Season> Seasons { get; set; } = new List<Season>();
    public ICollection<Character> Characters { get; set; } = new List<Character>();
    public ICollection<UserShow> UserShows { get; set; } = new List<UserShow>();
}
