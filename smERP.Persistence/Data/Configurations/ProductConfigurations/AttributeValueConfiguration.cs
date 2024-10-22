using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using smERP.Domain.Entities.Product;

namespace smERP.Persistence.Data.Configurations.ProductConfigurations;

public class AttributeValueConfiguration : IEntityTypeConfiguration<AttributeValue>
{
    public void Configure(EntityTypeBuilder<AttributeValue> builder)
    {
        builder.HasKey(x => new { x.AttributeId, x.Id });
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        //builder.HasMany(x => x.ProductInstanceAttributeValues).WithOne(x => x.AttributeValue).HasForeignKey(x => x.AttributeValueId);

        builder.Property(x => x.AttributeId).IsRequired();

        builder.OwnsOne(p => p.Value, value =>
        {
            value.Property(v => v.Arabic).IsRequired();
            value.Property(v => v.English).IsRequired();
            value.HasIndex(v => v.Arabic).IsClustered(false);
            value.HasIndex(v => v.English).IsClustered(false);
        });
    }
}
