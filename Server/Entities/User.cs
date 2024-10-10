namespace Entities;

public class User
{
    public User()
    {
        // Constructor for EF Core
    }
    public User(int id, string userName,string email, string password)
    {
        Id = id;
        UserName = userName;
        Password = password;
    }

    public int Id { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    
}