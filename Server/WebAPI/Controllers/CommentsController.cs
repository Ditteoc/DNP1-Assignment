using DTOs;  // Importér DTO'erne
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

    [HttpPost]
    public async Task<IActionResult> CreateComment([FromBody] CreateCommentDTO requestComment)
    {
        if (requestComment == null)
        {
            return BadRequest();
        }

        // Tjek, om brugeren og posten eksisterer
        var user = await _userRepository.GetSingleAsync(requestComment.UserId);
        if (user == null)
        {
            return NotFound($"User {requestComment.UserId} not found");
        }

        var post = await _postRepository.GetSingleAsync(requestComment.PostId);
        if (post == null)
        {
            return NotFound($"Post {requestComment.PostId} not found");
        }

        // Map CreateCommentDTO til en ny Comment entity
        var newComment = new Comment
        {
            Body = requestComment.Body,
            UserId = requestComment.UserId,
            PostId = requestComment.PostId
        };

        var createdComment = await _commentRepository.AddAsync(newComment);

        // Map Comment entity til CommentDTO og returner
        var commentDto = new CommentDTO
        {
            Id = createdComment.Id,
            Body = createdComment.Body,
            UserName = user.UserName,  // Brugernavn fra bruger, der skrev kommentaren
            PostId = createdComment.PostId
        };

        return CreatedAtAction(nameof(GetCommentById), new { id = commentDto.Id }, commentDto);
    }
    
    // Get api/comments - Returner en liste af kommentarer
    [HttpGet]
    public async Task<IActionResult> GetManyComments(int? postId, int? userId)
    {
        var comments = await _commentRepository.GetManyAsync();

        if (postId.HasValue)
        {
            comments = comments.Where(c => c.PostId == postId.Value);
        }

        if (userId.HasValue)
        {
            comments = comments.Where(c => c.UserId == userId.Value);
        }

        // Map Comment entity til CommentDTO
        var commentDtos = new List<CommentDTO>();
        foreach (var comment in comments)
        {
            var user = await _userRepository.GetSingleAsync(comment.UserId);
            var commentDto = new CommentDTO
            {
                Id = comment.Id,
                Body = comment.Body,
                UserName = user.UserName,
                PostId = comment.PostId
            };

            commentDtos.Add(commentDto);
        }

        return Ok(commentDtos);
    }

    // Get api/comments/{id} - Returner en enkelt kommentar
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCommentById(int id)
    {
        var comment = await _commentRepository.GetSingleAsync(id);
        if (comment == null)
        {
            return NotFound();
        }

        var user = await _userRepository.GetSingleAsync(comment.UserId);

        // Map Comment entity til CommentDTO
        var commentDto = new CommentDTO
        {
            Id = comment.Id,
            Body = comment.Body,
            UserName = user.UserName,
            PostId = comment.PostId
        };

        return Ok(commentDto);
    }

    

    // PUT: api/comments/{id} - Opdater en kommentar
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateComment(int id, [FromBody] CommentDTO commentDto)
    {
        if (commentDto == null || commentDto.Id != id)
        {
            return BadRequest();
        }

        var comment = await _commentRepository.GetSingleAsync(id);
        if (comment == null)
        {
            return NotFound();
        }

        // Opdater kommentarens indhold
        comment.Body = commentDto.Body;

        try
        {
            await _commentRepository.UpdateAsync(comment);
            return NoContent();
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }
    }

    // DELETE: api/comments/{id} - Slet en kommentar
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteComment(int id)
    {
        try
        {
            await _commentRepository.DeleteAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }
    }
}
