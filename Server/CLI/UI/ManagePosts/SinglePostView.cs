using Entities;
using RepositoryContracts;

namespace CLI.UI.ManagePosts;

public class SinglePostView
{
    private readonly IRepository<Post> _postRepository; // Dependency for handling posts
    private readonly IRepository<Comment> _commentRepository; // Dependency for handling comments

    // Constructor to inject the repositories
    public SinglePostView(IRepository<Post> postRepository,
        IRepository<Comment> commentRepository)
    {
        _postRepository = postRepository;
        _commentRepository = commentRepository;
    }

    // Method to display a single post along with its comments
    public async Task GetSingleAsync(int postId)
    {
        // Retrieve the post based on postId from the repository
        var post = await _postRepository.GetSingleAsync(postId);
        if (post == null)
        {
            Console.WriteLine($"Post {postId} not found");
            return;
        }
        
        // Display the post details
        Console.WriteLine($"Post ID: {post.Id}");
        Console.WriteLine($"Title: {post.Title}");
        Console.WriteLine($"Body: {post.Body}");
        Console.WriteLine($"Comments:");
        
        // Retrieve all comments for the given postId and display them
        var comments = (await _commentRepository.GetManyAsync()).Where(c => c.PostId == postId);
        foreach (var comment in comments)
        {
            // Display each comment's body and the associated user ID
            Console.WriteLine($" Comment: {comment.Body}, User: {comment.UserId} ");
        }
    }
}