// PostDTO.cs
public class PostDTO
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    public string UserName { get; set; }  // Author's username
    public  List<CommentDTO> Comments { get; set; } = new List<CommentDTO>();  // Optional, based on whether comments are included
}
