using System.ComponentModel.DataAnnotations;

namespace DTOs;

public class CreateUserDTO // To receive data from the client, when a new user is created 
{
    [Required]
    public string UserName { get; set; }
    
    [Required]
    public string Password { get; set; }
}