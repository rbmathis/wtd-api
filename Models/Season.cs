namespace WillTheyDie.Api.Models;

public class Season
{
    public int Id { get; set; }
    public int ShowId { get; set; }
    public int SeasonNumber { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public Show Show { get; set; } = null!;
    public ICollection<Episode> Episodes { get; set; } = new List<Episode>();
}
