using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using topCv.Domain.Entities.Commons;

namespace topCv.Infrastructure.Configuration.Commons
{
    public class ResumeTemplateVariantThemeConfiguration : IEntityTypeConfiguration<ResumeTemplateVariantTheme>
    {
        public void Configure(EntityTypeBuilder<ResumeTemplateVariantTheme> builder)
        {
            builder.ToTable("ResumeTemplateVariantThemes");

            builder.HasKey(x => new { x.VariantId, x.ThemePresetId });

            builder.Property(x => x.SortOrder).HasDefaultValue(0).IsRequired();

            builder.HasIndex(x => x.ThemePresetId);

            builder.HasOne(x => x.Variant)
                .WithMany(x => x.VariantThemes)
                .HasForeignKey(x => x.VariantId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.ThemePreset)
                .WithMany(x => x.VariantThemes)
                .HasForeignKey(x => x.ThemePresetId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
