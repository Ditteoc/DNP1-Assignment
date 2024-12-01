using DTOs;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/posts")]
public class PostsController : ControllerBase
{
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<Post> _postRepository;
    private readonly IRepository<Comment> _commentRepository;

    public PostsController(
        IRepository<User> userRepository,
        IRepository<Post> postRepository,
        IRepository<Comment> commentRepository)
    {
        _userRepository = userRepository;
        _postRepository = postRepository;
        _commentRepository = commentRepository;
    }

    [HttpPost]
    public async Task<IActionResult> CreatePost([FromBody] CreatePostDTO requestPost)
    {
        try
        {
            Console.WriteLine($"Received CreatePost request: Title={requestPost.Title}, Body={requestPost.Body}, UserId={requestPost.UserId}");

            if (requestPost == null)
                return BadRequest("Invalid post data.");

            var user = await _userRepository.GetSingleAsync(requestPost.UserId);
            if (user == null)
            {
                Console.WriteLine($"User with ID {requestPost.UserId} not found.");
                return NotFound($"User with ID {requestPost.UserId} not found.");
            }

            var newPost = new Post(0, requestPost.Title, requestPost.Body, requestPost.UserId);
            var createdPost = await _postRepository.AddAsync(newPost);

            var postDto = new PostDTO
            {
                Id = createdPost.Id,
                Title = createdPost.Title,
                Body = createdPost.Body,
                UserName = user.Username,
                Comments = new List<CommentDTO>()
            };

            return CreatedAtAction(nameof(GetPostById), new { id = postDto.Id }, postDto);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in CreatePost: {ex.Message}");
            return StatusCode(500, "An error occurred while creating the post.");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPosts()
    {
        try
        {
            var posts = await _postRepository.GetManyAsync();

            var postDtos = posts.Select(post => new PostDTO
            {
                Id = post.Id,
                Title = post.Title,
                Body = post.Body,
                UserName = post.User?.Username ?? "Unknown",
                Comments = post.Comments.Select(c => new CommentDTO
                {
                    Id = c.Id,
                    Body = c.Body,
                    PostId = c.PostId,
                    UserName = c.User?.Username ?? "Unknown"
                }).ToList()
            });

            return Ok(postDtos);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetAllPosts: {ex.Message}");
            return StatusCode(500, "An error occurred while retrieving posts.");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPostById(int id)
    {
        try
        {
            var post = await _postRepository.GetSingleAsync(id);
            if (post == null)
                return NotFound("Post not found.");

            var user = await _userRepository.GetSingleAsync(post.UserId);
            var comments = (await _commentRepository.GetManyAsync())
                .Where(c => c.PostId == id)
                .Select(async c => new CommentDTO
                {
                    Id = c.Id,
                    Body = c.Body,
                    PostId = c.PostId,
                    UserName = (await _userRepository.GetSingleAsync(c.UserId))?.Username ?? "Unknown"
                }).Select(t => t.Result).ToList();

            var postDto = new PostDTO
            {
                Id = post.Id,
                Title = post.Title,
                Body = post.Body,
                UserName = user?.Username ?? "Unknown",
                Comments = comments
            };

            return Ok(postDto);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetPostById: {ex.Message}");
            return StatusCode(500, "An error occurred while retrieving the post.");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePost(int id, [FromBody] UpdatePostDTO postDto)
    {
        try
        {
            if (postDto == null || id != postDto.Id)
                return BadRequest("Invalid post data.");

            var post = await _postRepository.GetSingleAsync(id);
            if (post == null)
                return NotFound("Post not found.");

            post.Title = postDto.Title;
            post.Body = postDto.Body;

            await _postRepository.UpdateAsync(post);
            return NoContent();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in UpdatePost: {ex.Message}");
            return StatusCode(500, "An error occurred while updating the post.");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePost(int id)
    {
        try
        {
            var post = await _postRepository.GetSingleAsync(id);
            if (post == null)
                return NotFound($"Post with ID {id} not found.");

            await _postRepository.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in DeletePost: {ex.Message}");
            return StatusCode(500, "An error occurred while deleting the post.");
        }
    }
}
