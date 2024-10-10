using DTOs;  // Importér dine DTO'er
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

    // Opret en ny post
    [HttpPost]
    public async Task<IActionResult> CreatePost([FromBody] CreatePostDTO requestPost)
    {
        if (requestPost == null)
        {
            return BadRequest();
        }

        // Check, om brugeren eksisterer
        var user = await _userRepository.GetSingleAsync(requestPost.UserId);
        if (user == null)
        {
            return NotFound($"User {requestPost.UserId} not found");
        }

        // Map CreatePostDTO til en ny Post entity
        var newPost = new Post
        {
            Title = requestPost.Title,
            Body = requestPost.Body,
            UserId = requestPost.UserId
        };

        var createdPost = await _postRepository.AddAsync(newPost);

        // Opret et PostDTO objekt til at returnere den oprettede post
        var postDto = new PostDTO
        {
            Id = createdPost.Id,
            Title = createdPost.Title,
            Body = createdPost.Body,
            UserName = user.UserName,
            Comments = new List<CommentDTO>()  // Tom liste ved oprettelse
        };

        return CreatedAtAction(nameof(GetPostById), new { id = postDto.Id }, postDto);
    }
    
    // Get api/posts - Returner en liste af PostDTO'er
    [HttpGet("search")]
    public async Task<IActionResult> GetManyPosts(string? title, int? userId, string? userName)
    {
        var posts = await _postRepository.GetManyAsync();

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

        // Hent data og konverter til PostDTO
        var postDtos = new List<PostDTO>();
        foreach (var post in posts)
        {
            var user = await _userRepository.GetSingleAsync(post.UserId);

            var postDto = new PostDTO
            {
                Id = post.Id,
                Title = post.Title,
                Body = post.Body,
                UserName = user.UserName,
                Comments = new List<CommentDTO>()  // Som tom liste
            };

            postDtos.Add(postDto);
        }

        return Ok(postDtos);
    }

    // Get api/posts/{id} - Returner en enkelt PostDTO
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPostById(int id)
    {
        var post = await _postRepository.GetSingleAsync(id);
        if (post == null)
        {
            return NotFound();
        }

        var user = await _userRepository.GetSingleAsync(post.UserId);

        // Opret PostDTO og returner det
        var postDto = new PostDTO
        {
            Id = post.Id,
            Title = post.Title,
            Body = post.Body,
            UserName = user.UserName,
            Comments = new List<CommentDTO>()  // Ingen kommentarer i dette tilfælde
        };

        return Ok(postDto);
    }
    

    // PUT: api/posts/{id} - Opdatering af post
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] PostDTO postDto)
    {
        if (postDto == null || postDto.Id != id)
        {
            return BadRequest();
        }

        var post = await _postRepository.GetSingleAsync(id);
        if (post == null)
        {
            return NotFound();
        }

        // Opdater post entiteten
        post.Title = postDto.Title;
        post.Body = postDto.Body;

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
