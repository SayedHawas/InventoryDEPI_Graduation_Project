namespace smERP.Domain.Entities.InventoryTransaction;

public abstract class Discount
{
    public string DiscountType { get; set; } = null!;
    public decimal Value { get; set; }

    public abstract decimal Apply(decimal amount);
}

