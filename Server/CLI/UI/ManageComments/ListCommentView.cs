using Entities;
using RepositoryContracts;

namespace CLI.UI.ManageComments;

public class ListCommentView
{
    private readonly IRepository<Comment> _commentRepository;

    public ListCommentView(IRepository<Comment> commentRepository)
    {
        _commentRepository = commentRepository;
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
                Console.WriteLine($"ID: {comment.Id}, Content: {comment.Body}");
            }
        }
    }
}