using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace movies.Auth;

public class JwtSettings
{
    // Properties configured with private set can still be modified by a configuration binder.
    public string SecretKey { get; private set; } = string.Empty;
    public string Issuer { get; private set; } = string.Empty;
    public string Audience { get; private set; } = string.Empty;
    public int ExpiryMinutes { get; private set; }

    // Custom Getter property that returns a new symmetric security key
    // This executes everytime we access this property
    public SymmetricSecurityKey SecurityKey
    {
        get { return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey)); }
    }

    // Another custom getter, but written with a lambda property, which is perfect for a read only property.
    public TokenValidationParameters TokenValidationParameters => new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = Issuer,
        ValidAudience = Audience,
        IssuerSigningKey = SecurityKey
    };
}