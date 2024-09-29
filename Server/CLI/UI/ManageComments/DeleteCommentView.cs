using Entities;
using RepositoryContracts;

namespace CLI.UI.ManageComments;

public class DeleteCommentView
{
    private readonly IRepository<Comment> _commentRepository;

    public DeleteCommentView(IRepository<Comment> commentRepository)
    {
        _commentRepository = commentRepository;
    }

    public async Task DeleteAsync()
    {
        Console.WriteLine("\nEnter the Comment ID to delete:");

        if (int.TryParse(Console.ReadLine(), out int commentId))
        {
            var comment = await _commentRepository.GetSingleAsync(commentId);

            if (comment == null)
            {
                Console.WriteLine($"Comment with ID {commentId} not found.");
                return;
            }

            await _commentRepository.DeleteAsync(commentId);
            Console.WriteLine($"Comment with ID {commentId} deleted successfully.");
        }
        else
        {
            Console.WriteLine("Invalid Comment ID. Please enter a valid number.");
        }
    }
}