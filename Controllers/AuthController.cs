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
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, "testuser"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())

        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(_jwt.ExpiryMinutes),
            signingCredentials: creds
        );

        return Ok(new JwtSecurityTokenHandler().WriteToken(token));
    }
}