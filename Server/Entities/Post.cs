namespace Entities;

public class Post
{
    private Post()
    { 
        // Constructor for EF Core
    }

    public Post(int id, string title, string body, int userId)
    {
        Id = id;
        Title = title;
        Body = body;
        UserId = userId;
    }

    public int Id { get; private set; }
    public string Title { get; set; }
    public string Body { get; set; } = null!;
    public int UserId { get; set; }

    public User User { get; private set; } = null!;
    public List<Comment> Comments { get; set; } = new List<Comment>();
}