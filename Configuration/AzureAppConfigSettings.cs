namespace WillTheyDie.Api.Configuration;

public class AzureAppConfigSettings
{
    public const string SectionName = "AzureAppConfiguration";
    
    public string ConnectionString { get; set; } = string.Empty;
    public bool Enabled { get; set; } = false;
    public int RefreshIntervalSeconds { get; set; } = 30;
    public string[] WatchedKeys { get; set; } = Array.Empty<string>();
}
