using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using smERP.Domain.Entities.ExternalEntities;

namespace smERP.Persistence.Data.Configurations.ExternalEntitiesConfigurations;

internal class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.HasKey(x => x.Id);

        builder.OwnsOne(p => p.Name, name =>
        {
            name.Property(n => n.Arabic).IsRequired();
            name.Property(n => n.English).IsRequired();
            name.HasIndex(n => n.Arabic).IsClustered(false);
            name.HasIndex(n => n.English).IsClustered(false);
        });

        builder.OwnsMany(p => p.Addresses, address =>
        {
            address.WithOwner();
            address.HasKey(x => x.Id);
            address.ToTable("SupplierAddresses");
            address.Property(n => n.Country).IsRequired();
            address.Property(n => n.State).IsRequired();
            address.Property(n => n.City).IsRequired();
            address.Property(n => n.Street).IsRequired();
            address.Property(n => n.PostalCode).IsRequired();
            address.Property(n => n.Comment);
        });

        builder.HasMany(x => x.SuppliedProducts).WithOne(x => x.Supplier).HasForeignKey(x => x.SupplierId);
    }
}
