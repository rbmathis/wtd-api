using WillTheyDie.Api.DTOs.Characters;
using WillTheyDie.Api.DTOs.Episodes;

namespace WillTheyDie.Api.DTOs.Shows;

public class ShowDetailDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string CurrencyName { get; set; } = string.Empty;
    public string CurrencySymbol { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public decimal InitialBalance { get; set; }
    public List<SeasonDto> Seasons { get; set; } = new();
    public List<CharacterDto> Characters { get; set; } = new();
}

public class SeasonDto
{
    public int Id { get; set; }
    public int ShowId { get; set; }
    public int SeasonNumber { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<EpisodeDto> Episodes { get; set; } = new();
}
