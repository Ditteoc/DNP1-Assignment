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
            var existingUser = await _userRepository.GetSingleAsync(userId);
            if (existingUser != null)
            {
                Console.WriteLine("Enter new Username: ");
                string newUsername = Console.ReadLine();

                var duplicateUser = (await _userRepository.GetManyAsync())
                    .FirstOrDefault(u => u.Username == newUsername && u.Id != userId);
                if (duplicateUser != null)
                {
                    Console.WriteLine("Username already exists. Please enter a different username.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(newUsername))
                {
                    Console.WriteLine("Username cannot be empty.");
                    return;
                }

                Console.WriteLine("Enter new Password: ");
                string newPassword = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(newPassword))
                {
                    Console.WriteLine("Password cannot be empty.");
                    return;
                }

                // Skab en ny instans af User med opdaterede værdier
                User updatedUser = new User(
                    existingUser.Id,        // Behold samme ID
                    newUsername,            // Nyt brugernavn
                    existingUser.Name,      // Behold eksisterende navn
                    existingUser.Email,     // Behold eksisterende email
                    newPassword             // Nyt password
                );

                await _userRepository.UpdateAsync(updatedUser);
                Console.WriteLine("User has been updated successfully.");
            }
            else
            {
                Console.WriteLine($"User with ID: {userId} does not exist.");
            }
        }
        else
        {
            Console.WriteLine("Invalid User ID. Please try again.");
        }
    }
}
