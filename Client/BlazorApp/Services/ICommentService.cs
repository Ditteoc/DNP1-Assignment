using DTOs;

namespace BlazorApp.Services;

public interface ICommentService
{
    Task<CommentDTO> AddCommentAsync(CreateCommentDTO request);
    Task<IEnumerable<CommentDTO>> GetCommentsByPostIdAsync(int postId);
    Task DeleteCommentAsync(int id);
}