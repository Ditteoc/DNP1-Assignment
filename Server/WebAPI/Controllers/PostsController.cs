using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]

public class PostsController : ControllerBase
{
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<Post> _postRepository;

    public PostsController(IRepository<User> userRepository,
        IRepository<Post> postRepository)
    {
        _userRepository = userRepository;
        _postRepository = postRepository;
    }
    
    // Get api/posts
    [HttpGet]
    public async Task<IActionResult> GetManyPosts(string? title, int? userId, string? userName)
    {
        var posts = await _postRepository.GetManyAsync(); // Use await for async method

        if (!string.IsNullOrWhiteSpace(title))
        {
            posts = posts.Where(p => p.Title.Contains(title, StringComparison.OrdinalIgnoreCase));
        }

        if (userId.HasValue)
        {
            posts = posts.Where(p => p.UserId == userId.Value);
        }

        if (!string.IsNullOrWhiteSpace(userName))
        {
            var user = (await _userRepository.GetManyAsync()).FirstOrDefault(u => u.UserName == userName);
            if (user != null)
            {
                posts = posts.Where(p => p.UserId == user.Id);
            }
            else
            {
                return NotFound($"User '{userName}' not found");
            }
        }

        var filteredPosts = posts.ToList();
        return Ok(filteredPosts);
    }
    
    // Get api/post/{id}
    // GetSingleAsync method
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPostById(int id)
    {
        var post = _postRepository.GetSingleAsync(id);
        if (post == null)
        {
            return NotFound();
        }

        return Ok(post);
    }
    
    // Add/(Create posts
    [HttpPost]
    public async Task<IActionResult> CreatePost([FromBody] Post post)
    {
        if (post == null)
        {
            return BadRequest();
        }
        
        // Check if the user exists
        var user = _userRepository.GetSingleAsync(post.UserId);
        if (user == null)
        {
            return NotFound($"User {post.UserId} not found");
        }
        
        var createPost = await _postRepository.AddAsync(post);
        return CreatedAtAction(nameof(GetSingleAsync), new { id = createPost.Id },
            createPost);
    }
    
    // Get post by ID
    // Get api/posts/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetSingleAsync(int id)
    {
        var post = await _postRepository.GetSingleAsync(id);
        if (post == null)
        {
            return NotFound();
        }

        return Ok(post);
    }
    
    
    
    // PUT: api/posts/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Post post)
    {
        if (post == null || post.Id != id)
        {
            return BadRequest();
        }

        try
        {
            await _postRepository.UpdateAsync(post);
            return NoContent();
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }
    }

    // DELETE: api/posts/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _postRepository.DeleteAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }
    }
}
