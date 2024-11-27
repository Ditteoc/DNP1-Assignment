using CLI.UI.ManageComments;
using CLI.UI.ManagePosts;
using CLI.UI.ManageUsers;
using Entities;
using RepositoryContracts;

namespace CLI.UI;

public class CliApp
{
    private readonly ListUserView _listUserView;
    private readonly CreateUserView _createUserView;
    private readonly UpdateUserView _updateUserView; // Tilføjelse af UpdateUserView
    private readonly DeleteUserView _deleteUserView;

    private readonly ListPostView _listPostView;
    private readonly CreatePostView _createPostView;
    private readonly DeletePostView _deletePostView;

    private readonly ListCommentView _listCommentView;
    private readonly CreateCommentView _createCommentView;
    private readonly DeleteCommentView _deleteCommentView;

    public CliApp(
        IRepository<User> userRepository, 
        IRepository<Post> postRepository, 
        IRepository<Comment> commentRepository)
    {
        _listUserView = new ListUserView(userRepository);
        _createUserView = new CreateUserView(userRepository);
        _updateUserView = new UpdateUserView(userRepository); // Initialisering af UpdateUserView
        _deleteUserView = new DeleteUserView(userRepository);

        _listPostView = new ListPostView(postRepository);
        _createPostView = new CreatePostView(postRepository, userRepository);
        _deletePostView = new DeletePostView(postRepository);

        _listCommentView = new ListCommentView(commentRepository, userRepository);
        _createCommentView = new CreateCommentView(userRepository, postRepository, commentRepository);
        _deleteCommentView = new DeleteCommentView(commentRepository);
    }

    public async Task StartAsync()
    {
        while (true)
        {
            Console.WriteLine("\nCLI Application Menu:");
            Console.WriteLine("Choose an option by selecting a number: ");
            Console.WriteLine("1. Add new User");
            Console.WriteLine("2. List all Users");
            Console.WriteLine("3. Update User"); // Tilføjet mulighed for at opdatere brugere
            Console.WriteLine("4. Delete User");
            Console.WriteLine("5. Add new Post");
            Console.WriteLine("6. List all Posts");
            Console.WriteLine("7. Delete Post");
            Console.WriteLine("8. Add new Comment");
            Console.WriteLine("9. List all Comments");
            Console.WriteLine("10. Delete Comment");
            Console.WriteLine("0. Exit");

            string option = Console.ReadLine();
            switch (option)
            {
                case "1":
                    await _createUserView.AddAsync();
                    break;
                case "2":
                    _listUserView.ListUsers();
                    break;
                case "3": // Tilføjelse af opdateringsmetoden
                    await _updateUserView.UpdateAsync();
                    break;
                case "4":
                    await _deleteUserView.DeleteAsync();
                    break;
                case "5":
                    await _createPostView.AddAsync();
                    break;
                case "6":
                    _listPostView.ListPosts();
                    break;
                case "7":
                    await _deletePostView.DeleteAsync();
                    break;
                case "8":
                    await _createCommentView.AddAsync();
                    break;
                case "9":
                    _listCommentView.ListComments();
                    break;
                case "10":
                    await _deleteCommentView.DeleteAsync();
                    break;
                case "0":
                    Console.WriteLine("Exiting...");
                    return;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }
}
