namespace WillTheyDie.Api.Models;

public class UserShow
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ShowId { get; set; }
    public decimal CurrencyBalance { get; set; } = 1000m; // Starting balance
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public User User { get; set; } = null!;
    public Show Show { get; set; } = null!;
}
