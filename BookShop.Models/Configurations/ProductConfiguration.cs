using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookShop.Models.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            // Table Configuration
            builder.ToTable("Products", "MasterSchema");

            // Primary Key
            builder.HasKey(p => p.Id);

            // Properties Configuration
            builder.Property(p => p.Id)
                .ValueGeneratedOnAdd();

            builder.Property(p => p.Title)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(p => p.Description)
                .HasMaxLength(250);

            builder.Property(p => p.Author)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(p => p.Price)
                .IsRequired()
                .HasColumnName("BookPrice")
                .HasPrecision(18, 2); // للتحكم في دقة الأرقام العشرية

            // Foreign Key Configuration
            builder.Property(p => p.CategoryId)
                .IsRequired();

            // Relationship Configuration
            builder.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes for Performance
            builder.HasIndex(p => p.CategoryId)
                .HasDatabaseName("IX_Products_CategoryId");

            builder.HasIndex(p => p.Title)
                .HasDatabaseName("IX_Products_Title");
        }
    }
}