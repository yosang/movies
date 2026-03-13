using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace movies.Auth;

public class JwtSettings
{
    // Properties configured can only be set once initiated, which makes this instance immutable and readonly
    public string SecretKey { get; init; } = string.Empty;
    public string Issuer { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public int ExpiryMinutes { get; init; }

    // Custom Getter property that returns a new symmetric security key
    // This executes everytime we access this property 
    // This SymetricSecurityKey type (wrapper) is simply as a container of bytes that a cryptographic mechanism can work with
    public SymmetricSecurityKey SecurityKey
    {
        get { return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey)); }
    }

    // Another custom getter, but written with a lambda property, which is perfect for a read only property.
    public TokenValidationParameters TokenValidationParameters => new TokenValidationParameters
    {
        ValidateIssuer = true, // Checks that the issuer on the token mat ches the one defined in settings, prevents tokens issued by other services
        ValidateAudience = true, // Same purpose as ValidateIssuer, prevents a token ment for one audience to be used by another, useful for specific consumers, not necessery for an open API
        ValidateLifetime = true, // Checks that the token has not expired and is still valid
        ValidateIssuerSigningKey = true, // Validates the JWT signature which is the IssuerSigningKey
        ValidIssuer = Issuer, // Used in conjuction with ValidateIssuer
        ValidAudience = Audience, // Used in conjuction with ValidateAudience
        IssuerSigningKey = SecurityKey // Assigns the IssuerSigningKey, which is a symmetric key (HMAC) wrapper of secretkey
    };
}