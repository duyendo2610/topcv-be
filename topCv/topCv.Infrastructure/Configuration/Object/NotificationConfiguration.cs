using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using topCv.Domain.Entities.Obj;

namespace topCv.Infrastructure.Configuration.Obj
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("Notifications");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserId).IsRequired();
            builder.HasIndex(x => x.UserId);

            builder.Property(x => x.Type).IsRequired();

            builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Body).HasColumnType("text");

            builder.Property(x => x.IsRead).HasDefaultValue(false).IsRequired();
            builder.Property(x => x.CreatedAt).IsRequired();
        }
    }
}
