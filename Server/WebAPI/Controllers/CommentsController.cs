using Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/posts/{postId}/comments")]

public class CommentsController : ControllerBase
{
    private readonly IRepository<Comment> _commentRepository;
    private readonly IRepository<Post> _postRepository;
    private readonly IRepository<User> _userRepository;

    public CommentsController(IRepository<Comment> commentRepository,
        IRepository<Post> postRepository, IRepository<User> userRepository)
    {
        _commentRepository = commentRepository;
        _postRepository = postRepository;
        _userRepository = userRepository;
    }
    
    // Get many comments
    // api/posts/{postId}/comments
    [HttpGet]
    public async Task<IActionResult> GetManyComments(int postId)
    {
        var comments = await _commentRepository.GetManyAsync(); // Use await on async method
        var filteredComments = comments.Where(c => c.PostId == postId).ToList(); // Add filtering by postId if needed
        return Ok(filteredComments);
    }
    // GET: api/posts/{postId}/comments/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSingle(int postId, int id)
        {
            var comment = await _commentRepository.GetSingleAsync(id);

            if (comment == null || comment.PostId != postId)
            {
                return NotFound();
            }

            return Ok(comment);
        }

        // POST: api/posts/{postId}/comments
        [HttpPost]
        public async Task<IActionResult> Create(int postId, [FromBody] Comment comment)
        {
            if (comment == null || comment.PostId != postId)
            {
                return BadRequest();
            }

            // Check if the post exists
            var post = await _postRepository.GetSingleAsync(postId);
            if (post == null)
            {
                return NotFound($"Post with ID {postId} not found.");
            }

            // Check if the user exists
            var user = await _userRepository.GetSingleAsync(comment.UserId);
            if (user == null)
            {
                return NotFound($"User with ID {comment.UserId} not found.");
            }

            var createdComment = await _commentRepository.AddAsync(comment);
            return CreatedAtAction(nameof(GetSingle), new { postId = postId, id = createdComment.Id }, createdComment);
        }

        // PUT: api/posts/{postId}/comments/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int postId, int id, [FromBody] Comment comment)
        {
            if (comment == null || comment.Id != id || comment.PostId != postId)
            {
                return BadRequest();
            }

            try
            {
                var existingComment = await _commentRepository.GetSingleAsync(id);
                if (existingComment == null || existingComment.PostId != postId)
                {
                    return NotFound();
                }

                // Check if the user exists
                var user = await _userRepository.GetSingleAsync(comment.UserId);
                if (user == null)
                {
                    return NotFound($"User with ID {comment.UserId} not found.");
                }

                await _commentRepository.UpdateAsync(comment);
                return NoContent();
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
        }

        // DELETE: api/posts/{postId}/comments/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int postId, int id)
        {
            try
            {
                var comment = await _commentRepository.GetSingleAsync(id);
                if (comment == null || comment.PostId != postId)
                {
                    return NotFound();
                }

                await _commentRepository.DeleteAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
        }
    }

    
