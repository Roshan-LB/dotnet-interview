using System.ComponentModel.DataAnnotations;

namespace TodoApi.DTOs
{
    public class CreateTodoRequest
    {
        [Required(ErrorMessage = "Title is required")]
        [MaxLength(200, ErrorMessage = "Title must not exceed 200 characters")]
        public string Title { get; set; } = string.Empty;
        [MaxLength(2000, ErrorMessage = "Description must not exceed 2000 characters")]
        public string? Description { get; set; } = null;
        public bool IsCompleted { get; set; } = false;
    }

    public class UpdateTodoRequest
    {
        [Required(ErrorMessage = "Title is required.")]
        [MaxLength(200, ErrorMessage = "Title must not exceed 200 characters.")]
        public string Title { get; set; } = string.Empty;
        [MaxLength(2000, ErrorMessage = "Description must not exceed 2000 characters.")]
        public string? Description { get; set; } = null;
        public bool IsCompleted { get; set; }
    }

    public class TodoResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; } = null;
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
