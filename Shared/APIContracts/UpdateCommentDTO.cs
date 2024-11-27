namespace DTOs;

public class UpdateCommentDTO
{
    public int Id { get; set; }           // ID for kommentaren, der opdateres
    public string Body { get; set; } = string.Empty; // Ny indhold for kommentaren
    public int PostId { get; set; }       // ID for den tilknyttede post
    public int UserId { get; set; }       // ID for den bruger, der ejer kommentaren
}