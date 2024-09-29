using Entities;
using RepositoryContracts;

namespace CLI.UI.ManagePosts;

public class DeletePostView
{
    private readonly IRepository<Post> _postRepository;

    public DeletePostView(IRepository<Post> postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task DeleteAsync()
    {
        Console.WriteLine("\nEnter the Post ID to delete:");

        if (int.TryParse(Console.ReadLine(), out int postId))
        {
            var post = await _postRepository.GetSingleAsync(postId);

            if (post == null)
            {
                Console.WriteLine($"Post with ID {postId} not found.");
                return;
            }

            await _postRepository.DeleteAsync(postId);
            Console.WriteLine($"Post with ID {postId} deleted successfully.");
        }
        else
        {
            Console.WriteLine("Invalid Post ID. Please enter a valid number.");
        }
    }
}