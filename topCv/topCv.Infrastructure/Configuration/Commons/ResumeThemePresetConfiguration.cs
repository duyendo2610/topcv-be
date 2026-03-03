using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using topCv.Domain.Entities.Commons;

namespace topCv.Infrastructure.Configuration.Commons
{
    public class ResumeThemePresetConfiguration : IEntityTypeConfiguration<ResumeThemePreset>
    {
        public void Configure(EntityTypeBuilder<ResumeThemePreset> builder)
        {
            builder.ToTable("ResumeThemePresets");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ThemeKey).HasMaxLength(80).IsRequired();
            builder.Property(x => x.Name).HasMaxLength(120).IsRequired();
            builder.Property(x => x.TemplateKey).HasMaxLength(40);
            builder.Property(x => x.ThemeJson).HasColumnType("nvarchar(max)").IsRequired();
            builder.Property(x => x.IsActive).HasDefaultValue(true).IsRequired();
            builder.Property(x => x.SortOrder).HasDefaultValue(0).IsRequired();
            builder.Property(x => x.CreatedAt).IsRequired();

            builder.HasIndex(x => x.ThemeKey).IsUnique();
            builder.HasIndex(x => x.TemplateKey);
        }
    }
}
