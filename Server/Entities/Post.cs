namespace Entities;

public class Post
{
    public Post()
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

    public int Id { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    public int UserId { get; set; }
    
    public List<Comment> Comments { get; set; } = new List<Comment>();
}