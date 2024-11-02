namespace DTOs;

public class UpdatePostDTO
{
    public required int Id { get; set; }            // ID for the post being updated
    public required string Title { get; set; }      // Title of the post, required
    public required string Content { get; set; }    // Content of the post, required
    public bool? IsPublished { get; set; }          // Optional: Defines if the post is published or a draft
}