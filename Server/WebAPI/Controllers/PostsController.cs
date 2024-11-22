using DTOs;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class PostsController(
    IRepository<User> userRepository,
    IRepository<Post> postRepository,
    IRepository<Comment> commentRepository)
    : ControllerBase
{
    
    // Opret en ny post
    [HttpPost]
    public async Task<IActionResult> CreatePost([FromBody] CreatePostDTO requestPost)
    {
        if (requestPost == null)
        {
            return BadRequest();
        }

        // Check, om brugeren eksisterer
        var user = await userRepository.GetSingleAsync(requestPost.UserId);
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

        var createdPost = await postRepository.AddAsync(newPost);

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
        var posts = await postRepository.GetManyAsync();

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
            var user = (await userRepository.GetManyAsync()).FirstOrDefault(u => u.UserName == userName);
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
            var user = await userRepository.GetSingleAsync(post.UserId);

            var postDto = new PostDTO
            {
                Id = post.Id,
                Title = post.Title,
                Body = post.Body,
                UserName = user?.UserName ?? "Unknown",
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
        // Fetch the post
        var post = await postRepository.GetSingleAsync(id);
        if (post == null)
        {
            return NotFound();
        }

        // Fetch comments from the comments.json file
        var comments = await commentRepository.GetManyAsync();
        var postComments = comments.Where(c => c.PostId == id).Select(comment =>
        {
            var user = userRepository.GetSingleAsync(comment.UserId).Result;
            return new CommentDTO
            {
                Id = comment.Id,
                Body = comment.Body,
                PostId = comment.PostId,
                UserName = user?.UserName ?? "Unknown"
            };
        }).ToList();

        // Map the post to PostDTO
        var user = await userRepository.GetSingleAsync(post.UserId);
        var postDto = new PostDTO
        {
            Id = post.Id,
            Title = post.Title,
            Body = post.Body,
            UserName = user?.UserName ?? "Unknown",
            Comments = postComments // Associate comments with the post
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

        var post = await postRepository.GetSingleAsync(id);
        if (post == null)
        {
            return NotFound();
        }

        // Opdater post entiteten
        post.Title = postDto.Title;
        post.Body = postDto.Body;

        try
        {
            await postRepository.UpdateAsync(post);
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
            await postRepository.DeleteAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllPosts()
    {
        var posts = await postRepository.GetManyAsync();

        // Convert to DTOs
        var postDtos = new List<PostDTO>();
        foreach (var post in posts)
        {
            var user = await userRepository.GetSingleAsync(post.UserId);

            var postDto = new PostDTO
            {
                Id = post.Id,
                Title = post.Title,
                Body = post.Body,
                UserName = user?.UserName ?? "Unknown",
                Comments = new List<CommentDTO>() // Return empty comments for now
            };

            postDtos.Add(postDto);
        }

        return Ok(postDtos);
    }
}