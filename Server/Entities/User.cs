namespace Entities;

public class User
{
    public User()
    {
        // Constructor for EF Core
    }
    public User(int id, string userName, string name, string email, string password)
    {
        Id = id;
        UserName = userName;
        Name = name;
        Email = email;
        Password = password;
    }

    public int Id { get; set; }
    public string UserName { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}