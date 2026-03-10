using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using movies.Auth;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly JwtSettings _jwt;

    // DI constructor
    public AuthController(JwtSettings jwtSettings)
    {
        _jwt = jwtSettings;
    }

    [HttpPost("token")]
    public IActionResult GenerateToken()
    {
        // Array containing claims
        // Claims a simply pieces of information ahbout the user or entity being authenticated
        // Here we are just creating a subject claim with "testuser" and a random unique ID for the JTI (unique identifier to prevent token reuse)
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, "testuser"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.SecretKey)); // Instance of secretKey wrapped in a SysmetricSecurityKey (can be used for both signing and verification)
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256); // Instance of credentials using key and an algorithm

        // The token creation
        var token = new JwtSecurityToken(
            issuer: _jwt.Issuer, // Who is issuing the token (defined in appsettings.json)
            audience: _jwt.Audience, // Who is the token intended for (defined in appsettings.json)
            claims: claims, // specific user information (claims)
            expires: DateTime.Now.AddMinutes(_jwt.ExpiryMinutes), // Token expiration
            signingCredentials: creds // The signature for the token
        );

        return Ok(new JwtSecurityTokenHandler().WriteToken(token)); // Token serialization (a JWT string that the client receives)
    }
}