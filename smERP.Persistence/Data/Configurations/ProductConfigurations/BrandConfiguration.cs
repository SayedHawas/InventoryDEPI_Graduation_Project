using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using smERP.Domain.Entities.Product;

namespace smERP.Persistence.Data.Configurations.ProductConfigurations;

public class BrandConfiguration : IEntityTypeConfiguration<Brand>
{
    public void Configure(EntityTypeBuilder<Brand> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasMany(d => d.Products).WithOne(p => p.Brand)
            .HasForeignKey(d => d.BrandId);

        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder
            .OwnsOne(p => p.Name, w =>
            {
                w.WithOwner();
                w.Property(wt => wt.Arabic);
                w.Property(wt => wt.English);
                w.HasIndex(wt => wt.Arabic).IsClustered(false);
                w.HasIndex(wt => wt.English).IsClustered(false);
            });
    }
}
