using System.Text.Json;
using DTOs;

namespace BlazorApp.Services;

public class HttpUserService : IUserService
{
    private readonly HttpClient _client;

    public HttpUserService(HttpClient client)
    {
        _client = client;
    }

    public async Task<UserDTO> AddUserAsync(CreateUserDTO request)
    {
        try
        {
            HttpResponseMessage response = await _client.PostAsJsonAsync("api/users", request);
            response.EnsureSuccessStatusCode();

            string responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<UserDTO>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        }
        catch (HttpRequestException ex)
        {
            Console.Error.WriteLine($"HTTP Request Error: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.Error.WriteLine($"Inner Exception: {ex.InnerException.Message}");
            }
            throw; // Genudkastning af undtagelsen for at sikre, at den håndteres højere oppe i kaldstakken
        }
    }

    public async Task<UserDTO> GetUserAsync(int id)
    {
        try
        {
            var user = await _client.GetFromJsonAsync<UserDTO>($"api/users/{id}");
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
            return await _client.GetFromJsonAsync<IEnumerable<UserDTO>>("api/users") ?? new List<UserDTO>();
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
            var response = await _client.PutAsJsonAsync($"api/users/{id}", request);
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
            var response = await _client.DeleteAsync($"api/users/{id}");
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error deleting user with ID {id}: {ex.Message}");
            throw;
        }
    }
    
    public async Task<UserDTO?> LoginAsync(LoginRequestDTO loginRequest)
    {
        try
        {
            var response = await _client.PostAsJsonAsync("api/auth/login", loginRequest);
            if (response.IsSuccessStatusCode)
            {
                var userDto = await response.Content.ReadFromJsonAsync<UserDTO>();
                return userDto;
            }
            else
            {
                Console.Error.WriteLine("Login failed");
                return null;
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error logging in: {ex.Message}");
            return null;
        }
    }

}
