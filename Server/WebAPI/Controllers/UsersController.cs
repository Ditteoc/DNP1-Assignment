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
   
   // Create a new user 
   // Post: Send data to the server to create a new resource
   [HttpPost]
   public async Task<IActionResult> CreateUser([FromBody] User user)
   {
      try
      {
         if (string.IsNullOrWhiteSpace(user.UserName))
         {
            return BadRequest(new{error = "User name is required"});
         }
         
         var users = await _userRepository.GetManyAsync(); // Await the asynchronous call

         var existingUser = users.FirstOrDefault(u => u.UserName == user.UserName);
         if (existingUser != null)
         {
            return Conflict(new { error = "User already exists" });
         }

         var newUser = await _userRepository.AddAsync(user);
         return CreatedAtAction(nameof(GetUser), new { id = newUser.Id }, newUser); // Ensure a return statement is present
      }
      catch (Exception e)
      {
         Console.WriteLine($"Error occured in creating new user: {e.Message}");
         return StatusCode(404, "The requested resource could not be found on the server");
      }
     
   }
   
   // Get a singe user by ID
   [HttpGet("{id}")]
   public async Task<IActionResult> GetUser(int id)
   {
      try
      {
         var user = await _userRepository.GetSingleAsync(id);
         if (user == null)
         {
            return NotFound("User not found");
         }

         return Ok(user);
      }
      catch (Exception e)
      {
         Console.WriteLine($"Error occured in GetUser: {e.Message}");
         return StatusCode(404, "The requested resource could not be found on the server");
      }
   }
   
   // Get all users (with optional search parameter)
   [HttpGet]
   public async Task<IActionResult> GetManyUsers([FromQuery] string? search)
   {
      try
      {
         var users = await _userRepository.GetManyAsync();
         var filteredUsers = users;

         if (!string.IsNullOrWhiteSpace(search))
         {
            filteredUsers = users.Where(u => u.UserName.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
         }

         return Ok(filteredUsers);
      }
      catch (Exception e)
      {
         Console.WriteLine($"Error occured in GetManyUsers: {e.Message}");
         return StatusCode(404,
            "The requested resource could not be found on the server");
      }
   }

   
   // Update an existing user
   [HttpPut("{id}")]
   public async Task<IActionResult> updateUser(int id,
      [FromBody] User user)
   {
      if (id != user.Id)
      {
         return BadRequest("User id is invalid");
      }

      try
      {
         await _userRepository.UpdateAsync(user);
         return NoContent();
      }
      catch (Exception e)
      {
         Console.WriteLine($"Error occured in updateUser: {e.Message}");
         return StatusCode(404,
            "The requested resource could not be found on the server");
      }
   }

   [HttpDelete("{id}")]
      public async Task<IActionResult> DeleteUser(int id)
      {
         try
         {
            await _userRepository.DeleteAsync(id);
            return NoContent();
         }
         catch (Exception e)
         {
            Console.WriteLine($"Error occured in DeleteUser: {e.Message}");
            return StatusCode(404, "The requested resource could not be found on the server");
         }
      }
   }
