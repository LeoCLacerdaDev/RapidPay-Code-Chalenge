namespace RbModels.Configuration;

public record JwtConfig
{
    public string ValidAudience { get; set; } = null!;
    public string ValidIssuer { get; set; } = null!;
    public string Secret { get; set; } = null!;
};