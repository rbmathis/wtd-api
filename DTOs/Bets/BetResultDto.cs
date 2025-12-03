namespace WillTheyDie.Api.DTOs.Bets;

public class BetResultDto
{
    public int BetId { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public decimal NewBalance { get; set; }
}
