using DTOs;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IRepository<User> _userRepository;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IRepository<User> userRepository, ILogger<AuthController> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginRequest)
    {
        if (string.IsNullOrWhiteSpace(loginRequest.Username) || string.IsNullOrWhiteSpace(loginRequest.Password))
        {
            return BadRequest(new { error = "Username and password must not be empty" });
        }

        _logger.LogInformation($"Login attempt for username: {loginRequest.Username}");

        // Fetch the user using GetSingleAsync
        var user = await _userRepository.GetSingleAsync(u => u.Username == loginRequest.Username);

        if (user == null)
        {
            _logger.LogWarning($"Login failed for username: {loginRequest.Username} - Username not found.");
            return Unauthorized(new { error = "Username is incorrect" });
        }

        if (user.Password != loginRequest.Password)
        {
            _logger.LogWarning($"Login failed for username: {loginRequest.Username} - Incorrect password.");
            return Unauthorized(new { error = "Password is incorrect" });
        }

        _logger.LogInformation($"Login succeeded for username: {loginRequest.Username}");
        var userDTO = new UserDTO
        {
            Id = user.Id,
            Username = user.Username,
            Name = user.Name,
            Email = user.Email
        };

        return Ok(userDTO);
    }
}