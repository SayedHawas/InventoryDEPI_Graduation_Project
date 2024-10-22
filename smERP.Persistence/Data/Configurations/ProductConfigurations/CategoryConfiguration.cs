using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using smERP.Domain.Entities;
using smERP.Domain.Entities.Product;

namespace smERP.Persistence.Data.Configurations.ProductConfigurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasOne(d => d.ParentCategory).WithMany(p => p.InverseParentCategory)
            .HasForeignKey(d => d.ParentCategoryId);

        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.IsLeaf).HasDefaultValue(true);
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
