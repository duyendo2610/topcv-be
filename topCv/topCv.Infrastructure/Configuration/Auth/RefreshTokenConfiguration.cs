using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using topCv.Domain.Entities.Auth;

namespace topCv.Infrastructure.Configuration.Auth
{
    internal class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshTokens");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserId).IsRequired();
            builder.HasIndex(x => x.UserId);

            builder.Property(x => x.TokenHash).IsRequired();
            builder.HasIndex(x => x.TokenHash).IsUnique();

            builder.Property(x => x.ExpiresAtUtc).IsRequired();
            builder.Property(x => x.RevokedAtUtc);
            builder.Property(x => x.CreatedAtUtc).IsRequired();
        }
    }
}
