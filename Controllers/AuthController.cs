using Microsoft.AspNetCore.Mvc;
using movies.Auth;
using movies.DTOs;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Generates a JWT token upon successful login
    /// </summary>
    /// <param name="request"></param>
    /// <response code="200">Token generated</response>
    /// <response code="401">Unable to validate user credentials</response>
    [HttpPost("/login")]
    [ProducesResponseType(typeof(LoginResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(LoginRequestDTO request)
    {
        var validLogin = await _authService.ValidateUser(request.Username, request.Password);

        if (!validLogin) return Unauthorized("Invalid credentials");

        var user = await _authService.GetUserBytUserName(request.Username);

        return Ok(new LoginResponseDTO { Token = _authService.GenerateToken(user!) });
    }

    /// <summary>
    /// Registerts a new user
    /// </summary>
    /// <param name="request"></param>
    /// <response code="200">Registration successful</response>
    /// <response code="400">Registration failed</response>
    [HttpPost("/register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(RegisterRequestDTO request)
    {
        var registered = await _authService.RegisterUser(request.Username, request.Password);

        if (!registered) return BadRequest("Registration failed");

        return Ok("Registration successful");
    }
}