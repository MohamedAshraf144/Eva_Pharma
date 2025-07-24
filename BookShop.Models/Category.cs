using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookShop.Models
{
    public class Category
    {
        public int Id { get; set; }

        public string CatName { get; set; } = string.Empty;

        public int CatOrder { get; set; }

        [NotMapped]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public bool IsDeleted { get; set; } = false;
    }
}