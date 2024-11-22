using DTOs;

namespace BlazorApp.Services;

public interface IPostService
{
    Task<PostDTO> CreatePostAsync(CreatePostDTO request);
    Task<PostDTO> GetPostAsync(int id);
    Task<IEnumerable<PostDTO>> GetPostsAsync();
    Task UpdatePostAsync(int id, UpdatePostDTO request);
    Task DeletePostAsync(int id);
    Task AddPostAsync(CreatePostDTO post);
}