namespace BookShop.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public decimal Price { get; set; } // Changed to decimal for better precision

        // Foreign Key
        public int CategoryId { get; set; }

        // Navigation Property
        public Category Category { get; set; } = null!;
    }
}