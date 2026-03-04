using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using topCv.Domain.Entities.Commons;

namespace topCv.Infrastructure.Configuration.Commons
{
    public class ResumeTemplateVariantConfiguration : IEntityTypeConfiguration<ResumeTemplateVariant>
    {
        public void Configure(EntityTypeBuilder<ResumeTemplateVariant> builder)
        {
            builder.ToTable("ResumeTemplateVariants");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.TemplateKey).HasMaxLength(40).IsRequired();
            builder.Property(x => x.VariantKey).HasMaxLength(80).IsRequired();
            builder.Property(x => x.Name).HasMaxLength(120).IsRequired();
            builder.Property(x => x.LayoutKey).HasMaxLength(80).IsRequired();
            builder.Property(x => x.Description).HasMaxLength(400);
            builder.Property(x => x.IsActive).HasDefaultValue(true).IsRequired();
            builder.Property(x => x.SortOrder).HasDefaultValue(0).IsRequired();
            builder.Property(x => x.CreatedAt).IsRequired();

            builder.HasIndex(x => x.TemplateKey);
            builder.HasIndex(x => x.VariantKey).IsUnique();
        }
    }
}
