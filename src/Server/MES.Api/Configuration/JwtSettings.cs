namespace MES.Api.Configuration;

public class JwtSettings
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "MES-API";
    public string Audience { get; set; } = "MES-Client";
    public string SecretKey { get; set; } = "YourSuperSecretKeyThatMustBeAtLeast32CharactersLong!";
    public int ExpiryMinutes { get; set; } = 480;
}
