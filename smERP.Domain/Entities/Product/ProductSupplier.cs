using smERP.Domain.Entities.ExternalEntities;

namespace smERP.Domain.Entities.Product;

public class ProductSupplier
{
    public int ProductId { get; set; }
    public int SupplierId { get; set; }
    public virtual Product Product { get; set; } = null!;
    public virtual Supplier Supplier { get; set; } = null!;
    public DateTime FirstTimeSupplied { get; set; }
    public DateTime LastTimeSupplied { get; set; }
}