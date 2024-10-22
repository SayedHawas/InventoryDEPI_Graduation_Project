using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Attribute = smERP.Domain.Entities.Product.Attribute;

namespace smERP.Persistence.Data.Configurations.ProductConfigurations;

public class AttributeConfiguration : IEntityTypeConfiguration<Attribute>
{
    public void Configure(EntityTypeBuilder<Attribute> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.OwnsOne(p => p.Name, name =>
        {
            name.Property(n => n.Arabic).IsRequired();
            name.Property(n => n.English).IsRequired();
            name.HasIndex(n => n.Arabic).IsClustered(false);
            name.HasIndex(n => n.English).IsClustered(false);
        });

        builder.HasMany(x => x.AttributeValues)
               .WithOne(x => x.Attribute)
               .HasForeignKey(x => x.AttributeId)
               .OnDelete(DeleteBehavior.Cascade);
    }
    //public void Configure(EntityTypeBuilder<Attribute> builder)
    //{
    //    builder.HasKey(x => x.Id);
    //    builder.HasMany(d => d.AttributeValues).WithOne(p => p.Attribute)
    //        .HasForeignKey(d => d.AttributeId);

    //    builder.OwnsMany(p => p.AttributeValues, w =>
    //    {
    //        w.HasKey(x => new { x.AttributeId, x.Id });
    //        w.HasOne(p => p.Attribute).WithMany(a => a.AttributeValues)
    //            .HasForeignKey(p => p.Id);
    //        w.OwnsOne(p => p.Value, w =>
    //        {
    //            w.WithOwner();
    //            w.Property(wt => wt.Arabic);
    //            w.Property(wt => wt.English);
    //            w.HasIndex(wt => wt.Arabic).IsClustered(false);
    //            w.HasIndex(wt => wt.English).IsClustered(false);
    //        });
    //    });

    //    builder.Property(x => x.Id).ValueGeneratedOnAdd();
    //    builder
    //        .OwnsOne(p => p.Name, w =>
    //        {
    //            w.WithOwner();
    //            w.Property(wt => wt.Arabic);
    //            w.Property(wt => wt.English);
    //            w.HasIndex(wt => wt.Arabic).IsClustered(false);
    //            w.HasIndex(wt => wt.English).IsClustered(false);
    //        });
    //}
    //public void Configure(EntityTypeBuilder<Attribute> builder)
    //{
    //    builder.HasKey(x => x.Id);
    //    builder.Property(x => x.Id).ValueGeneratedOnAdd();
    //    //builder.HasMany(x => x.AttributeValues).WithOne(x => x.Attribute).HasForeignKey("AttributeId");

    //    // Configure AttributeValues as owned entities
    //    builder.OwnsMany(p => p.AttributeValues, attributeValue =>
    //    {
    //        attributeValue.WithOwner().HasForeignKey("AttributeId");
    //        attributeValue.HasKey(x => x.Id);
    //        attributeValue.Property(x => x.Id).ValueGeneratedOnAdd();
    //        // Configure the Value property of AttributeValue
    //        attributeValue.OwnsOne(av => av.Value, value =>
    //        {
    //            value.WithOwner();
    //            value.Property(v => v.Arabic);
    //            value.Property(v => v.English);
    //            value.HasIndex(v => v.Arabic).IsClustered(false);
    //            value.HasIndex(v => v.English).IsClustered(false);
    //        });
    //    });

    //    // Configure the Name property of Attribute
    //    builder.OwnsOne(p => p.Name, name =>
    //    {
    //        name.WithOwner();
    //        name.Property(n => n.Arabic);
    //        name.Property(n => n.English);
    //        name.HasIndex(n => n.Arabic).IsClustered(false);
    //        name.HasIndex(n => n.English).IsClustered(false);
    //    });
    //}
    //public void Configure(EntityTypeBuilder<Attribute> builder)
    //{
    //    builder.HasKey(x => x.Id);
    //    builder.Property(x => x.Id).ValueGeneratedOnAdd();

    //    builder.OwnsMany(p => p.AttributeValues, attributeValue =>
    //    {
    //        attributeValue.WithOwner().HasForeignKey("AttributeId");
    //        attributeValue.Property<int>("Id").ValueGeneratedOnAdd();
    //        attributeValue.HasKey("Id", "AttributeId");

    //        attributeValue.OwnsOne(av => av.Value, value =>
    //        {
    //            value.WithOwner();
    //            value.Property(v => v.Arabic);
    //            value.Property(v => v.English);
    //            value.HasIndex(v => v.Arabic).IsClustered(false);
    //            value.HasIndex(v => v.English).IsClustered(false);
    //        });
    //    });

    //    builder.OwnsOne(p => p.Name, name =>
    //    {
    //        name.WithOwner();
    //        name.Property(n => n.Arabic);
    //        name.Property(n => n.English);
    //        name.HasIndex(n => n.Arabic).IsClustered(false);
    //        name.HasIndex(n => n.English).IsClustered(false);
    //    });
    //}
}
