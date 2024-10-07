using Entities;
using RepositoryContracts;

namespace CLI.UI.ManageUsers;

public class UpdateUserView
{
    private readonly IRepository<User> _userRepository;

    public UpdateUserView(IRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task UpdateAsync()
    {
        Console.WriteLine("\nEnter the User ID to update:");
        if (int.TryParse(Console.ReadLine(), out int userId))
        {
            var user = await _userRepository.GetSingleAsync(userId);
            if (user != null)
            {
                Console.WriteLine("Enter new Username: ");
                string newUsername = Console.ReadLine();

                var existingUser = (await _userRepository.GetManyAsync()).FirstOrDefault(u => u.UserName == newUsername && u.Id != userId);
                if (existingUser != null)
                {
                    Console.WriteLine("Username already exists, Please enter a different username");
                    return;
                }

                if (string.IsNullOrWhiteSpace(newUsername))
                {
                    Console.WriteLine("Username cannot be empty");
                    return;
                }
                
                Console.WriteLine("Enter new Password: ");
                string newpassword = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(newpassword))
                {
                    Console.WriteLine("Password cannot be empty");
                    return;
                }

                user.UserName = newUsername;
                user.Password = newpassword;
                
                await _userRepository.UpdateAsync(user);
                Console.WriteLine("User has been updated successfully");
            }
            else
            {
                Console.WriteLine($"User with ID: {userId} does not exist");
            }
        }
        else
        {
            Console.WriteLine("Invalid User ID. Please try again");
        }
    }
}