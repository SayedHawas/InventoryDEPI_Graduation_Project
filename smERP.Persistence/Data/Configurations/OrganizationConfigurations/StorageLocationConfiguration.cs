using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using smERP.Domain.Entities.Organization;

namespace smERP.Persistence.Data.Configurations.OrganizationConfigurations;

public class StorageLocationConfiguration : IEntityTypeConfiguration<StorageLocation>
{
    public void Configure(EntityTypeBuilder<StorageLocation> builder)
    {
        builder.HasKey(x => x.Id);

        builder.OwnsMany(x => x.StoredProductInstances, w =>
        {
            w.WithOwner().HasForeignKey(x => x.StorageLocationId);
            w.HasKey(x => new { x.StorageLocationId, x.ProductInstanceId });
            w.HasOne(x => x.ProductInstance).WithMany().HasForeignKey(x => x.ProductInstanceId);
            w.OwnsMany(x => x.Items, y =>
            {
                y.WithOwner().HasForeignKey(x => new { x.StorageLocationId, x.ProductInstanceId });
                y.HasKey(x => x.Id);
                y.HasIndex(x => x.SerialNumber).IsClustered(false);
            });
        });

        //builder.HasMany(x => x.AdjustmentTransactions).WithOne().HasForeignKey(x => x.StorageLocationId);

        //builder.HasMany(x => x.ProcurementTransactions).WithOne().HasForeignKey(x => x.StorageLocationId);

        //builder.HasMany(x => x.SalesTransactions).WithOne().HasForeignKey(x => x.StorageLocationId);
    }
}
