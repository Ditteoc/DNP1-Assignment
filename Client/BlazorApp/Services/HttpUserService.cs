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
        HttpResponseMessage response = await client.PostAsJsonAsync("users", request);
        string responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(responseContent);
        }

        return JsonSerializer.Deserialize<UserDTO>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    }

    public async Task<UserDTO> GetUserAsync(int id)
    {
        return await client.GetFromJsonAsync<UserDTO>($"users/{id}")!;
    }

    public async Task<IEnumerable<UserDTO>> GetUsersAsync()
    {
        return await client.GetFromJsonAsync<IEnumerable<UserDTO>>("users") ?? new List<UserDTO>();
    }

    public async Task UpdateUserAsync(int id, UpdateUserDTO request)
    {
        var response = await client.PutAsJsonAsync($"users/{id}", request);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteUserAsync(int id)
    {
        var response = await client.DeleteAsync($"users/{id}");
        response.EnsureSuccessStatusCode();
    }
}