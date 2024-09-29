using Entities;
using RepositoryContracts;

namespace CLI.UI.ManageComments;

public class ListCommentView
{
    private readonly IRepository<Comment> _commentRepository;
    private readonly IRepository<User> _userRepository;

    public ListCommentView(IRepository<Comment> commentRepository, IRepository<User> userRepository)
    {
        _commentRepository = commentRepository;
        _userRepository = userRepository;
    }

    public void ListComments()
    {
        var comments = _commentRepository.GetMany().ToList();

        if (comments.Count == 0)
        {
            Console.WriteLine("No comments found.");
        }
        else
        {
            Console.WriteLine("\nList of Comments:");
            foreach (var comment in comments)
            {
                // Get the user who wrote the comment
                var user = _userRepository.GetSingleAsync(comment.UserId).Result;

                // Display comment with user information
                string userInfo = user != null ? $"User: {user.UserName}" : "User: Unknown";
                Console.WriteLine($"ID: {comment.Id}, {userInfo}, Content: {comment.Body}");
            }
        }
    }
}
