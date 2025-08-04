using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookShop.Models.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            // Table Configuration
            builder.ToTable("Categories", "MasterSchema");

            // Primary Key
            builder.HasKey(c => c.Id);

            // Properties Configuration
            builder.Property(c => c.Id)
                .ValueGeneratedOnAdd();

            builder.Property(c => c.CatName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(c => c.CatOrder)
                .IsRequired();

            builder.Property(c => c.IsDeleted)
                .HasDefaultValue(false);

            builder.Property(c => c.IsActive)
                .HasDefaultValue(true);

            // CreatedDate Configuration - using Fluent API instead of Data Annotations
            builder.Property(c => c.CreatedDate)
                .HasDefaultValueSql("GETDATE()")
                .ValueGeneratedOnAdd();

            // Relationships Configuration
            builder.HasMany(c => c.Products)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}