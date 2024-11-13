using DTOs;
using Entities;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IRepository<User> _userRepository;

    public AuthController(IRepository<User> userRepository) 
    {
        _userRepository = userRepository;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginRequest)
    {
        var users = await _userRepository.GetManyAsync();
        var user = users.FirstOrDefault(u => u.UserName == loginRequest.Username);

        if (user == null)
        {
            return Unauthorized(new {error = "Username is incorrect"});
        }

        if (user.Password != loginRequest.Password)
        {
            return Unauthorized(new {error = "Password is incorrect"});
        }

        var userDTO = new UserDTO
        {
            Id = user.Id,
            Username = user.UserName,
            Name = user.Name,
            Email = user.Email
        };

        return Ok(userDTO);
    }
}