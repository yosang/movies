using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using movies.Context;
using movies.Models;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

using System.Text;

namespace movies.Auth;

public class AuthService
{
    private readonly MoviesContext _ctx;
    private readonly JwtSettings _jwt;

    public AuthService(MoviesContext context, JwtSettings jwt)
    {
        _ctx = context;
        _jwt = jwt;
    }

    public async Task<bool> ValidateUser(string username, string password)
    {
        var user = await GetUserByUsername(username);
        if (user == null) return false;

        // Creates an instance of PasswordHasher and Compares two hashed passwords
        var comparedHashedPw = new PasswordHasher<User>().VerifyHashedPassword(user, user.Password, password);

        return comparedHashedPw == PasswordVerificationResult.Success;
    }

    public async Task<bool> RegisterUser(string username, string password)
    {
        if (await CheckIfUserExists(username)) return false;

        // Creates a new user with username
        var user = new User { Username = username };

        // Adds a hashed password to the user
        // Creates an instance of PasswordHasher and Hashes a password which is added to the newly created user
        user.Password = new PasswordHasher<User>().HashPassword(user, password);

        _ctx.Users.Add(user);
        await _ctx.SaveChangesAsync();

        return true;
    }

    public async Task<User?> GetUserByUsername(string username) => await _ctx.Users.SingleOrDefaultAsync(e => e.Username == username);
    public async Task<bool> CheckIfUserExists(string username) => await _ctx.Users.AnyAsync(e => e.Username == username);

    public string GenerateToken(User user)
    {
        // The token creation
        var token = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: new List<Claim> // Claims a simply pieces of information ahbout the user or entity being authenticated
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                    },
            expires: DateTime.UtcNow.AddMinutes(_jwt.ExpiryMinutes), // Token expiration, current time + 60 min
            signingCredentials: new SigningCredentials(_jwt.SecurityKey, SecurityAlgorithms.HmacSha256) // The signature for the token
        );

        return new JwtSecurityTokenHandler().WriteToken(token); // Token serialization (a JWT string that the client receives)
    }
}
