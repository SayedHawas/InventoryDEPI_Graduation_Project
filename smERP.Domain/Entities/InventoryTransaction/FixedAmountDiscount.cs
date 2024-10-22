namespace smERP.Domain.Entities.InventoryTransaction;

public class FixedAmountDiscount : Discount
{
    public FixedAmountDiscount(decimal discountAmount)
    {
        DiscountType = "FixedAmount";
        Value = discountAmount;
    }

    public override decimal Apply(decimal amount)
    {
        return Math.Max(amount - Value, 0);
    }
}