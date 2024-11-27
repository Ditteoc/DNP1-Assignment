namespace Entities
{
    public class Comment
    {
        private Comment()
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

        public int Id { get; private set; }
        public int PostId { get; set; }
        public int UserId { get; set; }
        public string Body { get; set; } = string.Empty;

        public Post Post { get; private set; } = null!;
        public User User { get; private set; } = null!;
    }
}