using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using smERP.Domain.Entities.Product;
using smERP.Domain.ValueObjects;
using File = smERP.Domain.ValueObjects.File;

namespace smERP.Persistence.Data.Configurations.ProductConfigurations;

public class ProductInstanceConfiguration : IEntityTypeConfiguration<ProductInstance>
{
    public void Configure(EntityTypeBuilder<ProductInstance> builder)
    {
        //builder.HasMany(x => x.ProductInstanceAttributeValues).WithMany(x => x.ProductInstances);
        builder.HasIndex(x => x.Sku).IsClustered(false).IsUnique(true);

        builder.OwnsMany(x => x.Images, w =>
        {
            w.Property(x => x.Path).HasConversion(
                v => v,
                v => Image.Create(v).Path
            ); 
            w.WithOwner().HasForeignKey();
            w.HasKey(x => x.Path);
        });

        builder.OwnsMany(x => x.ProductInstanceAttributeValues, w =>
        {
            w.HasKey(x => new { x.ProductInstanceId, x.AttributeId, x.AttributeValueId });
            w.WithOwner().HasForeignKey(x => x.ProductInstanceId);
            w.HasOne(x => x.ProductInstance).WithMany(x => x.ProductInstanceAttributeValues).HasForeignKey(x => x.ProductInstanceId);
            w.HasOne(x => x.AttributeValue).WithMany().HasForeignKey(x => new { x.AttributeId, x.AttributeValueId });
            w.HasIndex(x => new { x.ProductInstanceId, x.AttributeId }).IsUnique(true).IsClustered(false);
            w.ToTable(tb => tb.UseSqlOutputClause(false));
        });

        //builder.OwnsMany(x => x.ProductInstanceAttributes, w =>
        //{
        //    w.WithOwner().HasForeignKey(x => x.ProductInstanceId);
        //    w.
        //});

        ////builder.HasMany(x => x.ProductInstanceAttributes).WithOne(x => x.ProductInstance).HasForeignKey(x => x.ProductInstanceId);

        //builder.OwnsMany(x => x.ProductInstanceAttributes, attribute =>
        //{
        //    attribute.WithOwner().HasForeignKey(x => x.ProductInstanceId);
        //    attribute.HasOne(x => x.AttributeValue).WithMany(x => x.ProductInstanceAttributes).HasForeignKey(x => x.AttributeValueId);
        //    attribute.HasOne(x => x.ProductInstance).WithMany(x => x.ProductInstanceAttributes).HasForeignKey(x => x.ProductInstanceId);
        //    attribute.HasKey(x => new { x.ProductInstanceId, x.AttributeValueId });
        //});
    }
}
