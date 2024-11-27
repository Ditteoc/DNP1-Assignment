using CLI.UI;
using Entities;
using FileRepositories;
using RepositoryContracts;

namespace CLI;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Starting CLI Application...");

        // Use the BaseInMemoryRepository<T> for User, Post, and Comment with proper getId and setId delegates
        IRepository<User> userRepository = new FileRepository<User>(
            "users.json",  // File path for storing user data
            user => user.Id  // Delegate to get the user Id
           
        );

        IRepository<Post> postRepository = new FileRepository<Post>(
            "posts.json",  // File path for storing post data
            post => post.Id // Delegate to get the post Id
 
        );

        IRepository<Comment> commentRepository = new FileRepository<Comment>(
            "comments.json",  // File path for storing comment data
            comment => comment.Id // Delegate to get the comment Id
        );

        // Pass the generic repositories to CliApp
        CliApp cliApp = new CliApp(userRepository, postRepository, commentRepository);
        await cliApp.StartAsync();
    }
}