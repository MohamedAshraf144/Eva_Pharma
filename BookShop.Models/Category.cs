namespace BookShop.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string CatName { get; set; } = string.Empty;
        public int CatOrder { get; set; }
        public bool IsDeleted { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; }

        // Navigation Properties
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}