using Entities;
using RepositoryContracts;

namespace CLI.UI.ManageUsers;

public class ListUserView
{
    private readonly IRepository<User> _userRepository;

    public ListUserView(IRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public void ListUsers()
    {
        var users = _userRepository.GetMany().ToList();

        if (users.Any())
        {
            Console.WriteLine("\nListing users: ");
            foreach (var user in users)
            {
                Console.WriteLine($"ID: {user.Id}, Username: {user.UserName}");
            }
        }
        else
        {
            Console.WriteLine("No users found.");
        }
    }
}