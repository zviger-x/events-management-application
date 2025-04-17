using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.EntityConfigurations
{
    internal class UserTransactionConfiguration : IEntityTypeConfiguration<UserTransaction>
    {
        public void Configure(EntityTypeBuilder<UserTransaction> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserId).IsRequired();
            builder.Property(x => x.EventId).IsRequired();
            builder.Property(x => x.EventName).IsRequired(false);
            builder.Property(x => x.SeatRow).IsRequired();
            builder.Property(x => x.SeatNumber).IsRequired();
            builder.Property(x => x.Amount).IsRequired();
            builder.Property(x => x.TransactionDate).IsRequired();
        }
    }
}
