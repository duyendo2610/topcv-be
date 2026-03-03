using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using topCv.Domain.Entities.Commons;
using topCv.Domain.Enums;

namespace topCv.Infrastructure.Configuration.Commons
{
    public class ResumeConfiguration : IEntityTypeConfiguration<Resume>
    {
        public void Configure(EntityTypeBuilder<Resume> builder)
        {
            builder.ToTable("Resumes");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserId).IsRequired();
            builder.HasIndex(x => x.UserId);

            builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
            builder.Property(x => x.TemplateKey)
                .HasMaxLength(40)
                .HasDefaultValue("simple")
                .IsRequired();
            builder.Property(x => x.TemplateVariantId);
            builder.HasIndex(x => x.TemplateVariantId);
            builder.Property(x => x.ThemePresetId);
            builder.HasIndex(x => x.ThemePresetId);
            builder.Property(x => x.ThemeJson)
                .HasColumnType("nvarchar(max)")
                .HasDefaultValue("{}")
                .IsRequired();

            builder.Property(x => x.Visibility)
                .HasDefaultValue((ResumeVisibility)0);
            builder.Property(x => x.IsDefault).HasDefaultValue(false).IsRequired();

            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt);

            builder.HasOne(x => x.TemplateVariant)
                .WithMany(x => x.Resumes)
                .HasForeignKey(x => x.TemplateVariantId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(x => x.ThemePreset)
                .WithMany(x => x.Resumes)
                .HasForeignKey(x => x.ThemePresetId)
                .OnDelete(DeleteBehavior.SetNull);

            // Child: sections/files có thể cascade theo schema note
            builder.HasMany(x => x.Sections)
                .WithOne(x => x.Resume)
                .HasForeignKey(x => x.ResumeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Files)
                .WithOne(x => x.Resume)
                .HasForeignKey(x => x.ResumeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
