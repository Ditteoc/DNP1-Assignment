using System.ComponentModel.DataAnnotations;

namespace DTOs
{
    public class CreatePostDTO
    {
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Body is required.")]
        public string Body { get; set; }

        [Required(ErrorMessage = "User ID is required.")]
        public int UserId { get; set; }
    }
}