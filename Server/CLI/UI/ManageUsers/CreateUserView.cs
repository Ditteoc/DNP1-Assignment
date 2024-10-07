using Entities;
using RepositoryContracts;


namespace CLI.UI.ManageUsers;

public class CreateUserView
{
    private readonly IRepository<User> _userRepository;

    public CreateUserView(IRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task AddAsync()
    {
        Console.WriteLine("\nCreate new user account");
        Console.WriteLine("Enter username: ");
        string userName = Console.ReadLine();

        var users = await _userRepository.GetManyAsync() ?? Enumerable.Empty<User>();
        var existingUser = users.FirstOrDefault(u => u.UserName == userName);
        if (existingUser != null)
        {
            Console.WriteLine(
                "Username already exists. Please enter a different username.");
            return;
        }

        if (string.IsNullOrWhiteSpace(userName))
        {
            Console.WriteLine("Username cannot be empty.");
            return;
        }
        
        Console.WriteLine("Enter password: ");
        string password = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(password))
        {
            Console.WriteLine("Password cannot be empty.");
            return;
        }

        User user = new User { UserName = userName, Password = password };

        await _userRepository.AddAsync(user);
        Console.WriteLine("User created successfully");
    }
}