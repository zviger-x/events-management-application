using DataAccess.Entities;
using DataAccess.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.EntityConfigurations
{
    internal class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.Surname).IsRequired();
            builder.Property(x => x.Email).IsRequired();
            builder.Property(x => x.PasswordHash).IsRequired();
            builder.Property(u => u.Role).IsRequired();

            builder.HasIndex(u => u.Email).IsUnique();

            // enum to string
            builder.Property(u => u.Role)
                .HasConversion(
                    v => v.ToString(),
                    v => (UserRoles)Enum.Parse(typeof(UserRoles), v)
                );
        }
    }
}
