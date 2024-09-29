using Entities;
using RepositoryContracts;

namespace CLI.UI.ManageUsers;

public class DeleteUserView
{
    private readonly IRepository<User> _userRepository;

    public DeleteUserView(IRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task DeleteAsync()
    {
        Console.WriteLine("\nEnter the User Id to delete");
        if (int.TryParse(Console.ReadLine(), out int userId))
        {
            await _userRepository.DeleteAsync(userId);
            Console.WriteLine($"User with ID {userId} deleted successfully");
        }
        else
        {
            Console.WriteLine($"User with ID {userId} not found");
        }
    }
}