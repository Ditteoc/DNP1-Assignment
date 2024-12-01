using System.Text.Json;
using DTOs;

namespace BlazorApp.Services;

public class HttpCommentService : ICommentService
{
    private readonly HttpClient client;

    public HttpCommentService(HttpClient client)
    {
        this.client = client;
    } 
        
    public async Task<CommentDTO> AddCommentAsync(CreateCommentDTO request)
    {
        try
        {
            string apiUrl = "comments"; // The endpoint you're calling
            string fullUrl = $"{client.BaseAddress}{apiUrl}";
            Console.WriteLine($"Sending request to: {fullUrl}");
            Console.WriteLine($"Payload: {JsonSerializer.Serialize(request)}");

            HttpResponseMessage response = await client.PostAsJsonAsync(apiUrl, request);
            string responseString = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"API Response: {response.StatusCode} - {responseString}");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"HTTP {response.StatusCode}: {responseString}");
            }

            return JsonSerializer.Deserialize<CommentDTO>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in AddCommentAsync: {ex.Message}");
            throw;
        }
    }



    public async Task<IEnumerable<CommentDTO>> GetCommentsByPostIdAsync(int postId)
    {
        return await client.GetFromJsonAsync<IEnumerable<CommentDTO>>($"comments?postId={postId}") ?? new List<CommentDTO>();
    }

    public async Task DeleteCommentAsync(int id)
    {
        HttpResponseMessage response = await client.DeleteAsync($"comments?id={id}");
        response.EnsureSuccessStatusCode();
    }
}