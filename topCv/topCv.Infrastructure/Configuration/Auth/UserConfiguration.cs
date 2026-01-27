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
    internal class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Email).IsRequired().HasMaxLength(256);

            builder.HasIndex(x => x.Email).IsUnique();

            builder.Property(x => x.PasswordHash).IsRequired().HasMaxLength(500);

            builder.Property(x => x.Role).IsRequired().HasMaxLength(50);

            builder.Property(x => x.IsActive).HasDefaultValue(true);

            builder.Property(x => x.CreatedAtUtc).IsRequired();

            builder.HasMany(x => x.RefreshTokens)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
