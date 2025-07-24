using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookShop.Models.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categories", "MasterSchema");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.CatName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(c => c.CatOrder)
                .IsRequired();

            builder.Property(c => c.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            
        }
    }
}