using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models
{
    public class TodoItem
    {
        [Key]
        public long Id { get; set; }

        [Required(ErrorMessage = "The title is required")]
        [StringLength(50, ErrorMessage = "The title must be between 1 and 50 characters long")]
        public string Title { get; set; } = default!;

        public string? Description { get; set; }
    }
}