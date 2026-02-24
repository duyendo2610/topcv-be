using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using topCv.Domain.Entities.Commons;

namespace topCv.Infrastructure.Configuration.Commons
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categories");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name).HasMaxLength(150).IsRequired();
            builder.HasIndex(x => x.Name).IsUnique();

            builder.HasOne(x => x.Parent)
                .WithMany(x => x.Children)
                .HasForeignKey(x => x.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.ParentId);
        }
    }
}