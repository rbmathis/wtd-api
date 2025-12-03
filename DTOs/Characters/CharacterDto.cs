namespace WillTheyDie.Api.DTOs.Characters;

public class CharacterDto
{
    public int Id { get; set; }
    public int ShowId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Actor { get; set; }
    public string? ImageUrl { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
