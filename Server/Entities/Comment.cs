namespace Entities
{
    public class Comment
    {
        public Comment()
        {
            // Constructor for EF Core
        }
        public Comment(int id, string body, int postId, Post post, int userId, User user)
        {
            Id = id;
            Body = body;
            PostId = postId;
            UserId = userId;
        }

        public int Id { get; set; }
        public string Body { get; set; }
        public int PostId { get; set; }
        public int UserId { get; set; }
        
    }
}