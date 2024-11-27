using Entities;
using RepositoryContracts;

namespace CLI.UI.ManagePosts;

public class CreatePostView
{
    private readonly IRepository<Post> _postRepository;
    private readonly IRepository<User> _userRepository;

    public CreatePostView(IRepository<Post> postRepository,
        IRepository<User> userRepository)
    {
        _postRepository = postRepository;
        _userRepository = userRepository;
    }

    public async Task AddAsync()
    {
        Console.WriteLine("Creating new post ");
        Console.WriteLine("Enter post title: ");
        string title = Console.ReadLine();
        
        Console.WriteLine("Enter post body: ");
        string body = Console.ReadLine();
        
        Console.WriteLine("Enter User Id to set post author: ");
        if (!int.TryParse(Console.ReadLine(), out int userId))
        {
            Console.WriteLine("Invalid User ID.");
            return;
        }

        var user = await _userRepository.GetSingleAsync(userId);
        if (user == null)
        {
            Console.WriteLine("User not found");
            return;
        }

        // Brug konstruktoren til at oprette en ny Post
        Post post = new Post(0, title, body, userId);

        // Tilføj posten til repository
        await _postRepository.AddAsync(post);
        Console.WriteLine("Post created successfully.");
    }
}