namespace WillTheyDie.Api.DTOs.Shows;

public class ShowDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string CurrencyName { get; set; } = string.Empty;
    public string CurrencySymbol { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
