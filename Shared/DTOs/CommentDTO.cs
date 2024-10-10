// CommentDTO.cs
public class CommentDTO
{
    public int Id { get; set; }
    public string Body { get; set; }
    public int PostId { get; set; }
    public string UserName { get; set; }  // User who made the comment
}
