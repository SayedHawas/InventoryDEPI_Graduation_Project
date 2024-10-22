namespace smERP.Infrastructure.Identity.Models;

public record JwtSettings
{
    public string Key { get; init; }
    public string Issuer { get; init; }
    public string Audience { get; init; }
    public double DurationInMinutes { get; init; }

    public JwtSettings()
    {
    }

    public JwtSettings(string key, string issuer, string audience, double durationInMinutes)
    {
        Key = key;
        Issuer = issuer;
        Audience = audience;
        DurationInMinutes = durationInMinutes;
    }
}
