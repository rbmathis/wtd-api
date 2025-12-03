namespace WillTheyDie.Api.DTOs.Episodes;

public class EpisodeDto
{
    public int Id { get; set; }
    public int SeasonId { get; set; }
    public int EpisodeNumber { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime? AirDate { get; set; }
    public bool IsBettingOpen { get; set; }
}
