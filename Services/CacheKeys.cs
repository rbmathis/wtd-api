namespace WillTheyDie.Api.Services;

public static class CacheKeys
{
    public const string ShowPrefix = "show:";
    public const string CharacterPrefix = "character:";
    public const string EpisodePrefix = "episode:";
    public const string SeasonPrefix = "season:";
    public const string UserShowPrefix = "usershow:";
    
    public static string Show(int showId) => $"{ShowPrefix}{showId}";
    public static string ShowsList() => $"{ShowPrefix}list";
    public static string Character(int characterId) => $"{CharacterPrefix}{characterId}";
    public static string CharactersByShow(int showId) => $"{CharacterPrefix}show:{showId}";
    public static string Episode(int episodeId) => $"{EpisodePrefix}{episodeId}";
    public static string Season(int seasonId) => $"{SeasonPrefix}{seasonId}";
    public static string UserShow(int userId, int showId) => $"{UserShowPrefix}{userId}:{showId}";
}
