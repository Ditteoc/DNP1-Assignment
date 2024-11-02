using System.Text.Json;
using DTOs;

namespace BlazorApp.Services;

public class HttpPostService : IPostService
{
    private readonly HttpClient client;

    public HttpPostService(HttpClient client)
    {
        this.client = client;
    }

    // Opretter et nyt indlæg
    public async Task<PostDTO> CreatePostAsync(CreatePostDTO request)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync("posts", request);
        string responseString = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(responseString);
        }
        
        return JsonSerializer.Deserialize<PostDTO>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    }

    // Henter et specifikt indlæg
    public async Task<PostDTO> GetPostAsync(int id)
    {
        HttpResponseMessage response = await client.GetAsync($"posts/{id}");
        string responseString = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(responseString);
        }
        
        return JsonSerializer.Deserialize<PostDTO>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    }

    // Henter alle indlæg
    public async Task<IEnumerable<PostDTO>> GetPostsAsync()
    {
        HttpResponseMessage reponse = await client.GetAsync("posts");
        string responseString = await reponse.Content.ReadAsStringAsync();

        if (!reponse.IsSuccessStatusCode)
        {
            throw new Exception(responseString);
        }
        
        return JsonSerializer.Deserialize<IEnumerable<PostDTO>>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    }

    // Opdaterer et eksisterende indlæg
    public async Task UpdatePostAsync(int id, UpdatePostDTO request)
    {
        HttpResponseMessage response = await client.PutAsJsonAsync($"posts/{id}", request);
        string responseString = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(responseString);
        }
    }

    // Sletter et specifikt indlæg
    public async Task DeletePostAsync(int id)
    {
        HttpResponseMessage response = await client.DeleteAsync($"posts/{id}");
        string responseString = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(responseString);
        }
    }
}