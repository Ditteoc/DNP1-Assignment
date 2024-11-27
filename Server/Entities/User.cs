namespace Entities;

public class User
{
    private User()
    {
        // Constructor for EF Core
    }

    public User(int id, string username, string name, string email,
        string password)
    {
        Id = id;
        Username = username;
        Name = name;
        Email = email;
        Password = password;
    }

    public int Id { get; private set; }
    public string Username { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; } 

    public List<Post> Posts { get; set; } = new();
    public List<Comment> Comments { get; set; } = new();
    
}
