using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using smERP.Domain.Entities.Product;

namespace smERP.Persistence.Data.Configurations.ProductConfigurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasMany(d => d.ProductInstances).WithOne(p => p.Product)
            .HasForeignKey(d => d.ProductId);

        builder.HasOne(d => d.Brand).WithMany(p => p.Products)
            .HasForeignKey(d => d.BrandId);

        builder.HasOne(d => d.Category).WithMany(p => p.Products)
            .HasForeignKey(d => d.CategoryId);

        builder.HasMany(x => x.ProductSuppliers).WithOne(x => x.Product)
            .HasForeignKey(x => x.ProductId);

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

        builder.ToTable(tb => tb.UseSqlOutputClause(false));
    }
}
