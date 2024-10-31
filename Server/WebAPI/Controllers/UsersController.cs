using DTOs;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly IRepository<User> _userRepository;

    public UsersController(IRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    // Opret en ny bruger 
    [HttpPost]
    public async Task<ActionResult<UserDTO>> AddUser([FromBody] CreateUserDTO requestUser)
    {
        try
        {
            // Validering af brugernavn og password
            if (string.IsNullOrWhiteSpace(requestUser.UserName))
            {
                return BadRequest(new { error = "User name is required" });
            }
            if (string.IsNullOrWhiteSpace(requestUser.Password))
            {
                return BadRequest(new { error = "Password is required" });
            }

            // Tjek om brugernavn allerede findes
            var users = await _userRepository.GetManyAsync();
            var existingUser = users.FirstOrDefault(u => u.UserName == requestUser.UserName);

            if (existingUser != null)
            {
                return Conflict(new { error = "User already exists" });
            }

            // Opret ny bruger entitet
            var user = new User
            {
                UserName = requestUser.UserName,
                Password = requestUser.Password // I en reel app skal adgangskoden hashes
            };

            var createdUser = await _userRepository.AddAsync(user);

            // Returner som UserDTO
            var dto = new UserDTO
            {
                Id = createdUser.Id,
                Username = createdUser.UserName
            };

            return CreatedAtAction(nameof(GetUser), new { id = dto.Id }, dto);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error occurred in creating new user: {e.Message}");
            return StatusCode(500, "An error occurred while creating the user.");
        }
    }

    // Hent en enkelt bruger baseret på ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
        var user = await _userRepository.GetSingleAsync(id);
        if (user == null)
        {
            return NotFound("User not found");
        }

        // Returner UserDTO
        var userDto = new UserDTO
        {
            Id = user.Id,
            Username = user.UserName
        };

        return Ok(userDto);
    }

    // Hent alle brugere (med valgfri søgeparameter)
    [HttpGet]
    public async Task<IActionResult> GetManyUsers([FromQuery] string? search)
    {
        try
        {
            var users = await _userRepository.GetManyAsync();

            // Filtrer brugere baseret på søgeparameter, hvis den er angivet
            if (!string.IsNullOrWhiteSpace(search))
            {
                users = users.Where(u => u.UserName.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            var usersDto = users.Select(user => new UserDTO
            {
                Id = user.Id,
                Username = user.UserName
            }).ToList();

            return Ok(usersDto);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error occurred in GetManyUsers: {e.Message}");
            return StatusCode(500, "An error occurred while retrieving users.");
        }
    }

    // Opdater en eksisterende bruger
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDTO userDto)
    {
        if (userDto == null || id != userDto.Id)
        {
            return BadRequest("User id is invalid or data is missing");
        }

        try
        {
            var existingUser = await _userRepository.GetSingleAsync(id);
            if (existingUser == null)
            {
                return NotFound("User not found");
            }

            // Opdater kun de nødvendige felter
            existingUser.UserName = userDto.Username ?? existingUser.UserName;
            if (!string.IsNullOrWhiteSpace(userDto.Password))
            {
                existingUser.Password = userDto.Password; // Adgangskode skal hashes
            }

            await _userRepository.UpdateAsync(existingUser);
            return NoContent();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error occurred in UpdateUser: {e.Message}");
            return StatusCode(500, "An error occurred while updating the user.");
        }
    }

    // Slet en bruger baseret på ID
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        try
        {
            var user = await _userRepository.GetSingleAsync(id);
            if (user == null)
            {
                return NotFound("User not found");
            }

            await _userRepository.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error occurred in DeleteUser: {e.Message}");
            return StatusCode(500, "An error occurred while deleting the user.");
        }
    }
}
