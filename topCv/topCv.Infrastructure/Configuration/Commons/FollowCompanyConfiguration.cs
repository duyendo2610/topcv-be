using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using topCv.Domain.Entities.Commons;

namespace topCv.Infrastructure.Configuration.Commons
{
    public class FollowCompanyConfiguration : IEntityTypeConfiguration<FollowCompany>
    {
        public void Configure(EntityTypeBuilder<FollowCompany> builder)
        {
            builder.ToTable("FollowCompanies");

            builder.HasKey(x => new { x.UserId, x.CompanyId });

            builder.Property(x => x.CreatedAt).IsRequired();

            builder.HasOne(x => x.User)
                .WithMany(x => x.FollowCompanies)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Company)
                .WithMany(x => x.Followers)
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}