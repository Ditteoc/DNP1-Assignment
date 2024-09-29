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
        private readonly DeleteUserView _deleteUserView;

        private readonly ListPostView _listPostView;
        private readonly CreatePostView _createPostView;
        private readonly DeletePostView _deletePostView;

        private readonly ListCommentView _listCommentView;
        private readonly CreateCommentView _createCommentView;
        private readonly DeleteCommentView _deleteCommentView;

        public CliApp(IRepository<User> userRepository, IRepository<Post> postRepository, IRepository<Comment> commentRepository)
        {
            _listUserView = new ListUserView(userRepository);
            _createUserView = new CreateUserView(userRepository);
            _deleteUserView = new DeleteUserView(userRepository);

            _listPostView = new ListPostView(postRepository);
            _createPostView = new CreatePostView(postRepository, userRepository);
            _deletePostView = new DeletePostView(postRepository);

            _listCommentView = new ListCommentView(commentRepository);
            _createCommentView = new CreateCommentView(userRepository, postRepository, commentRepository);  // Correct order
            _deleteCommentView = new DeleteCommentView(commentRepository);
        }

        public async Task StartAsync()
        {
            while (true)
            {
                Console.WriteLine("CLI Application Menu:");
                Console.WriteLine("Choose an option by selecting a number: ");
                Console.WriteLine("1. Add new User");
                Console.WriteLine("2. List all Users");
                Console.WriteLine("3. Delete User");
                Console.WriteLine("4. Add new Post");
                Console.WriteLine("5. List all Posts");
                Console.WriteLine("6. Delete Post");
                Console.WriteLine("7. Add new Comment");
                Console.WriteLine("8.List all Comments");
                Console.WriteLine("9. Delete Comment");
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
                    case "3":
                        await _deleteUserView.DeleteAsync();
                        break;
                    case "4":
                        await _createPostView.AddAsync();
                        break;
                    case "5":
                        _listPostView.ListPosts();
                        break;
                    case "6":
                        await _deletePostView.DeleteAsync();
                        break;
                    case "7":
                        await _createCommentView.AddAsync();
                        break;
                    case "8":
                        _listCommentView.ListComments();
                        break;
                    case "9":
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
