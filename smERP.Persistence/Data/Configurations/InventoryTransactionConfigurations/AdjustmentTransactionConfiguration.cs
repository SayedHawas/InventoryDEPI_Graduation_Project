using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using smERP.Domain.Entities.InventoryTransaction;

namespace smERP.Persistence.Data.Configurations.InventoryTransactionConfigurations;

public class AdjustmentTransactionConfiguration : IEntityTypeConfiguration<AdjustmentTransaction>
{
    public void Configure(EntityTypeBuilder<AdjustmentTransaction> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.TransactionDate).HasDefaultValueSql("GETUTCDATE()");

        builder.Property(x => x.IsTransactionProcessed);

        builder.HasOne(x => x.StorageLocation).WithMany(x => x.AdjustmentTransactions).HasForeignKey(x => x.StorageLocationId);

        builder.OwnsMany(x => x.Items, w =>
        {
            w.WithOwner().HasForeignKey(x => x.TransactionId);
            w.HasKey(x => new { x.TransactionId, x.Id });
            w.OwnsMany(y => y.InventoryTransactionItemUnits, k =>
            {
                k.WithOwner().HasForeignKey(z => new { z.TransactionId, z.TransactionItemId });
                k.HasKey(z => z.Id);
            });
        });
    }
}
