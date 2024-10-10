namespace DTOs;

public class UserDTO // Used to return data to the client when retrieving a user
{
    public int Id { get; set; } // Server generates this
    public string Username { get; set; } // Client sees this
    // password is excluded for security reasons (sensitive data)
}