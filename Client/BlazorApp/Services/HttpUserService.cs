using System.Text.Json;
using DTOs;

namespace BlazorApp.Services;

public class HttpUserService : IUserService
{
    private readonly HttpClient client;

    public HttpUserService(HttpClient client)
    {
        this.client = client;
    }

    public async Task<UserDTO> AddUserAsync(CreateUserDTO request)
    {
        try
        {
            HttpResponseMessage response = await client.PostAsJsonAsync("users", request);
            string responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to add user: {responseContent}");
            }

            return JsonSerializer.Deserialize<UserDTO>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error adding user: {ex.Message}");
            throw; // Rethrow exception to propagate the error if necessary
        }
    }

    public async Task<UserDTO> GetUserAsync(int id)
    {
        try
        {
            var user = await client.GetFromJsonAsync<UserDTO>($"users/{id}");
            if (user == null)
            {
                throw new Exception("User not found.");
            }
            return user;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error fetching user with ID {id}: {ex.Message}");
            throw;
        }
    }

    public async Task<IEnumerable<UserDTO>> GetUsersAsync()
    {
        try
        {
            return await client.GetFromJsonAsync<IEnumerable<UserDTO>>("users") ?? new List<UserDTO>();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error fetching users: {ex.Message}");
            return new List<UserDTO>(); // Return an empty list on failure
        }
    }

    public async Task UpdateUserAsync(int id, UpdateUserDTO request)
    {
        try
        {
            var response = await client.PutAsJsonAsync($"users/{id}", request);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error updating user with ID {id}: {ex.Message}");
            throw;
        }
    }

    public async Task DeleteUserAsync(int id)
    {
        try
        {
            var response = await client.DeleteAsync($"users/{id}");
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error deleting user with ID {id}: {ex.Message}");
            throw;
        }
    }
}
