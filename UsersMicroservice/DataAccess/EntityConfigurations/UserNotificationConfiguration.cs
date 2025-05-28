using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Enums;

namespace DataAccess.EntityConfigurations
{
    internal class UserNotificationConfiguration : IEntityTypeConfiguration<UserNotification>
    {
        public void Configure(EntityTypeBuilder<UserNotification> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserId).IsRequired();
            builder.Property(x => x.Message).IsRequired();
            builder.Property(x => x.DateTime).IsRequired();
            builder.Property(x => x.Status).IsRequired();

            // enum to string
            builder.Property(u => u.Status)
                .HasConversion(
                    v => v.ToString(),
                    v => (NotificationStatuses)Enum.Parse(typeof(NotificationStatuses), v)
                );
        }
    }
}
