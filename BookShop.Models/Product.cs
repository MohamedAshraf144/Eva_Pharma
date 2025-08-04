using System.ComponentModel.DataAnnotations;

namespace BookShop.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [MaxLength(50, ErrorMessage = "Title cannot exceed 50 characters")]
        public string Title { get; set; } = string.Empty;

        [MaxLength(250, ErrorMessage = "Description cannot exceed 250 characters")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Author is required")]
        [MaxLength(50, ErrorMessage = "Author cannot exceed 50 characters")]
        public string Author { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 10000, ErrorMessage = "Price must be between $0.01 and $10,000")]
        public decimal Price { get; set; }

        // Foreign Key
        [Required(ErrorMessage = "Category is required")]
        public int CategoryId { get; set; }

        // Navigation Property - مش required للـ Model Binding
        public Category? Category { get; set; }
    }
}