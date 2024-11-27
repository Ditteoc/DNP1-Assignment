using Entities;
using RepositoryContracts;

namespace CLI.UI.ManageComments;

public class CreateCommentView
{
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<Post> _postRepository;
    private readonly IRepository<Comment> _commentRepository;

    public CreateCommentView(IRepository<User> userRepository,
        IRepository<Post> postRepository, IRepository<Comment> commentRepository)
    {
        _userRepository = userRepository;
        _postRepository = postRepository;
        _commentRepository = commentRepository;
    }

    public async Task AddAsync()
    {
        // Input the post ID
        Console.WriteLine("Enter Post ID: ");
        if (!int.TryParse(Console.ReadLine(), out int postId))
        {
            Console.WriteLine("Invalid Post ID.");
            return;
        }

        // Check if the post exists
        var post = await _postRepository.GetSingleAsync(postId);
        if (post == null)
        {
            Console.WriteLine("Post not found.");
            return;
        }

        // Input the user ID
        Console.WriteLine("Enter User ID: ");
        if (!int.TryParse(Console.ReadLine(), out int userId))
        {
            Console.WriteLine("Invalid User ID.");
            return;
        }

        // Check if the user exists
        var user = await _userRepository.GetSingleAsync(userId);
        if (user == null)
        {
            Console.WriteLine("User not found.");
            return;
        }

        // Input the comment body
        Console.WriteLine("Enter Comment: ");
        string commentBody = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(commentBody))
        {
            Console.WriteLine("Comment cannot be empty.");
            return;
        }

        // Create the comment object
        Comment newComment = new Comment(0, commentBody, postId, post, userId, user);
        
        // Add the comment to the repository
        await _commentRepository.AddAsync(newComment);
        Console.WriteLine("Comment added successfully.");
    }
}
