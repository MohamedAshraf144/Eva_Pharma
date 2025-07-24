using BookShop.Models;
using Microsoft.EntityFrameworkCore;

namespace BookShop.DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Categories", "MasterSchema");
                entity.HasKey(c => c.Id);
                entity.Property(c => c.CatName).IsRequired().HasMaxLength(50);
                entity.Property(c => c.CatOrder).IsRequired();
                entity.Property(c => c.IsDeleted).IsRequired().HasDefaultValue(false);
                
            });
        }
    }
}