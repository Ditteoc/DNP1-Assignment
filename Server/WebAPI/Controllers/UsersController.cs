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
            // Sørg for, at brugernavn ikke er tomt eller null
            if (string.IsNullOrWhiteSpace(requestUser.UserName))
            {
                return BadRequest(new { error = "User name is required" });
            }

            // Hent alle brugere og tjek om brugernavn allerede findes
            var users = await _userRepository.GetManyAsync();
            var existingUser = users.FirstOrDefault(u => u.UserName == requestUser.UserName);

            if (existingUser != null)
            {
                return Conflict(new { error = "User already exists" });
            }

            // Opret en ny bruger entitet
            User user = new User
            {
                UserName = requestUser.UserName,
                Password = requestUser.Password // I en reel app skal adgangskoden hashes
            };

            // Tilføj brugeren til repository
            User createdUser = await _userRepository.AddAsync(user);

            // Konverter til UserDTO
            UserDTO dto = new UserDTO
            {
                Id = createdUser.Id,
                Username = createdUser.UserName
            };

            // Returner 201 Created med DTO og lokationen af den nye ressource
            return CreatedAtAction(nameof(GetUser), new { id = dto.Id }, dto);
        }
        catch (Exception e)
        {
            // Log fejlen og returnér 500 Internal Server Error
            Console.WriteLine($"Error occurred in creating new user: {e.Message}");
            return StatusCode(500, e.Message);
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

        // Returner UserDTO for at beskytte følsomme oplysninger
        var userDto = new UserDTO
        {
            Id = user.Id,
            Username = user.UserName // Sørg for, at dette matcher din DTO's definition
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
            var filteredUsers = users;

            // Filtrer brugere efter søgeparameter
            if (!string.IsNullOrWhiteSpace(search))
            {
                filteredUsers = users.Where(u => u.UserName.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Konverter til DTO'er for at beskytte følsomme oplysninger
            var usersDto = filteredUsers.Select(user => new UserDTO
            {
                Id = user.Id,
                Username = user.UserName
            }).ToList();

            return Ok(usersDto);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error occurred in GetManyUsers: {e.Message}");
            return StatusCode(404, "The requested resource could not be found on the server");
        }
    }

    // Opdater en eksisterende bruger
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDTO userDto)
    {
        if (id != userDto.Id)
        {
            return BadRequest("User id is invalid");
        }

        try
        {
            var existingUser = await _userRepository.GetSingleAsync(id);
            if (existingUser == null)
            {
                return NotFound("User not found");
            }

            // Opdater brugeroplysninger (kun de nødvendige felter)
            existingUser.UserName = userDto.Username;
            if (!string.IsNullOrEmpty(userDto.Password))
            {
                existingUser.Password = userDto.Password; // Igen, husk at hashe i virkeligheden
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
