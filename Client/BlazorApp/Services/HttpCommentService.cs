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
        HttpResponseMessage response = await client.PostAsJsonAsync("comments", request);
        string responseString = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(responseString);
        }
        
        return JsonSerializer.Deserialize<CommentDTO>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
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