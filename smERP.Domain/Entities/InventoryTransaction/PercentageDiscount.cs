namespace smERP.Domain.Entities.InventoryTransaction;


public class PercentageDiscount : Discount
{
    public PercentageDiscount(decimal percentage)
    {
        DiscountType = "Percentage";
        Value = percentage;
    }

    public override decimal Apply(decimal amount)
    {
        return amount * (1 - Value / 100);
    }
}