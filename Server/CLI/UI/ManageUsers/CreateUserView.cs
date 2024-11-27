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
        var existingUser = users.FirstOrDefault(u => u.Username == userName);
        if (existingUser != null)
        {
            Console.WriteLine("Username already exists. Please enter a different username.");
            return;
        }

        if (string.IsNullOrWhiteSpace(userName))
        {
            Console.WriteLine("Username cannot be empty.");
            return;
        }

        Console.WriteLine("Enter full name: ");
        string name = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("Name cannot be empty.");
            return;
        }

        Console.WriteLine("Enter email: ");
        string email = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(email))
        {
            Console.WriteLine("Email cannot be empty.");
            return;
        }

        Console.WriteLine("Enter password: ");
        string password = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(password))
        {
            Console.WriteLine("Password cannot be empty.");
            return;
        }

        // Brug korrekt konstruktor til at oprette en ny User
        User user = new User(0, userName, name, email, password);

        await _userRepository.AddAsync(user);
        Console.WriteLine("User created successfully.");
    }
}