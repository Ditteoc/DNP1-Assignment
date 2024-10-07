using Entities;
using RepositoryContracts;

namespace CLI.UI.ManageComments;

public class ListCommentView
{
    private readonly IRepository<Comment> _commentRepository;
    private readonly IRepository<User> _userRepository;

    public ListCommentView(IRepository<Comment> commentRepository,
        IRepository<User> userRepository)
    {
        _commentRepository = commentRepository;
        _userRepository = userRepository;
    }

    public async Task ListComments()
    {
        var comments = (await _commentRepository.GetManyAsync()).ToList();

        if (comments.Count == 0)
        {
            Console.WriteLine("No comments found.");
        }
        else
        {
            Console.WriteLine("\nList of Comments:");
            foreach (var comment in comments)
            {
                try
                {
                    var user =
                        await _userRepository.GetSingleAsync(comment.UserId);
                    string userInfo = user != null ? $"User: {user.UserName}"
                        : "User: Unknown";
                    Console.WriteLine(
                        $"ID: {comment.Id}, {userInfo}, Content: {comment.Body}");
                }
                catch (Exception)
                {
                    Console.WriteLine(
                        $"ID: {comment.Id}, User: Unknown, Content: {comment.Body}");
                }
            }
        }
    }
}

    
