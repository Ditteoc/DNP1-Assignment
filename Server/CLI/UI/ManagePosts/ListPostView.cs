using Entities;
using RepositoryContracts;

namespace CLI.UI.ManagePosts;

public class ListPostView
{
    private readonly IRepository<Post> _postRepository;

    public ListPostView(IRepository<Post> postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task ListPosts()
    {
        var posts = (await _postRepository.GetManyAsync()).ToList();
        if (posts.Count == 0)
        {
            Console.WriteLine("No posts found.");
            return;
        }

        Console.WriteLine("Listing all posts:");
        foreach (var post in posts)
        {
            Console.WriteLine($"ID: {post.Id}, Title: {post.Title}, Body: {post.Body}");
        }
    }
}