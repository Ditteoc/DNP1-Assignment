using DTOs;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentsController : ControllerBase
{
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<Post> _postRepository;
    private readonly IRepository<Comment> _commentRepository;

    public CommentsController(IRepository<User> userRepository,
        IRepository<Post> postRepository,
        IRepository<Comment> commentRepository)
    {
        _userRepository = userRepository;
        _postRepository = postRepository;
        _commentRepository = commentRepository;
    }

    // Opret en ny kommentar
    [HttpPost]
    public async Task<IActionResult> CreateComment([FromBody] CreateCommentDTO requestComment)
    {
        try
        {
            if (requestComment == null || string.IsNullOrWhiteSpace(requestComment.Body))
            {
                return BadRequest("Comment body cannot be null or empty.");
            }

            // Tjek, om brugeren og posten eksisterer
            var user = await _userRepository.GetSingleAsync(requestComment.UserId);
            if (user == null)
            {
                return NotFound($"User with ID {requestComment.UserId} not found.");
            }

            var post = await _postRepository.GetSingleAsync(requestComment.PostId);
            if (post == null)
            {
                return NotFound($"Post with ID {requestComment.PostId} not found.");
            }

            // Opret en ny kommentar
            var newComment = new Comment(
                id: 0, // ID håndteres normalt af databasen
                body: requestComment.Body,
                postId: requestComment.PostId,
                post: post, // Post-objektet hentet fra databasen
                userId: requestComment.UserId,
                user: user // User-objektet hentet fra databasen
            );

            var createdComment = await _commentRepository.AddAsync(newComment);

            // Returner den oprettede kommentar som DTO
            var commentDto = new CommentDTO
            {
                Id = createdComment.Id,
                Body = createdComment.Body,
                UserName = user.Username,
                PostId = createdComment.PostId
            };

            return CreatedAtAction(nameof(GetCommentById), new { id = commentDto.Id }, commentDto);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in CreateComment: {ex.Message}");
            return StatusCode(500, "An error occurred while creating the comment.");
        }
    }

    // Get api/comments - Returner en liste af kommentarer
    [HttpGet]
    public async Task<IActionResult> GetManyComments([FromQuery] int? postId, [FromQuery] int? userId)
    {
        try
        {
            var comments = await _commentRepository.GetManyAsync();

            // Filtrer baseret på `postId` og `userId`
            if (postId.HasValue)
            {
                comments = comments.Where(c => c.PostId == postId.Value);
            }

            if (userId.HasValue)
            {
                comments = comments.Where(c => c.UserId == userId.Value);
            }

            // Map kommentarerne til DTO'er
            var users = await _userRepository.GetManyAsync();
            var commentDtos = comments.Select(comment =>
            {
                var user = users.FirstOrDefault(u => u.Id == comment.UserId);
                return new CommentDTO
                {
                    Id = comment.Id,
                    Body = comment.Body,
                    UserName = user?.Username ?? "Unknown",
                    PostId = comment.PostId
                };
            }).ToList();

            return Ok(commentDtos);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetManyComments: {ex.Message}");
            return StatusCode(500, "An error occurred while retrieving comments.");
        }
    }

    // Get api/comments/{id} - Returner en enkelt kommentar
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCommentById(int id)
    {
        try
        {
            var comment = await _commentRepository.GetSingleAsync(id);
            if (comment == null)
            {
                return NotFound("Comment not found.");
            }

            var user = await _userRepository.GetSingleAsync(comment.UserId);

            // Returner kommentaren som DTO
            var commentDto = new CommentDTO
            {
                Id = comment.Id,
                Body = comment.Body,
                UserName = user?.Username ?? "Unknown",
                PostId = comment.PostId
            };

            return Ok(commentDto);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetCommentById: {ex.Message}");
            return StatusCode(500, "An error occurred while retrieving the comment.");
        }
    }

    // PUT: api/comments/{id} - Opdater en kommentar
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateComment(int id, [FromBody] UpdateCommentDTO commentDto)
    {
        try
        {
            if (commentDto == null || commentDto.Id != id)
            {
                return BadRequest("Invalid comment data or ID mismatch.");
            }

            var comment = await _commentRepository.GetSingleAsync(id);
            if (comment == null)
            {
                return NotFound("Comment not found.");
            }

            // Opdater kommentarens indhold
            comment.Body = commentDto.Body;

            await _commentRepository.UpdateAsync(comment);
            return NoContent();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in UpdateComment: {ex.Message}");
            return StatusCode(500, "An error occurred while updating the comment.");
        }
    }

    // DELETE: api/comments/{id} - Slet en kommentar
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteComment(int id)
    {
        try
        {
            var comment = await _commentRepository.GetSingleAsync(id);
            if (comment == null)
            {
                return NotFound("Comment not found.");
            }

            await _commentRepository.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in DeleteComment: {ex.Message}");
            return StatusCode(500, "An error occurred while deleting the comment.");
        }
    }
}
