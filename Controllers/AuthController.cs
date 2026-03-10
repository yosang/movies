using Microsoft.AspNetCore.Mvc;
using movies.Auth;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    // DI constructor
    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("/login")]
    public async Task<IActionResult> Login(LoginRequestDTO request)
    {
        var validLogin = await _authService.ValidateUser(request.Username, request.Password);

        if (!validLogin) return Unauthorized("Invalid credentials");

        var user = await _authService.GetUserBytUserName(request.Username);

        return Ok(new LoginResponseDTO { Token = _authService.GenerateToken(user!) });
    }

    [HttpPost("/register")]
    public async Task<IActionResult> Register(RegisterRequestDTO request)
    {
        var registered = await _authService.RegisterUser(request.Username, request.Password);

        if (!registered) return BadRequest("Registration failed");

        return Ok("Registration successful");
    }
}