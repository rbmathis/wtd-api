namespace WillTheyDie.Api.Configuration;

public class RedisSettings
{
    public const string SectionName = "Redis";
    
    public string ConnectionString { get; set; } = string.Empty;
    public bool Enabled { get; set; } = true;
    public int DefaultExpirationMinutes { get; set; } = 30;
    public string InstanceName { get; set; } = "WillTheyDie:";
}
