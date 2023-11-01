namespace Entities.ConfigurationModels;

public class JwtConfiguration
{
    public const string Key = "JwtSettings";
    public string? ValidIssuer { get; set; }
    public string? ValidAudience { get; set; }
    public string? Expires { get; set; }
}