
namespace smERP.Domain.Entities.InventoryTransaction;

public class AdjustmentTransaction : InventoryTransaction
{
    public AdjustmentTransaction(int storageLocationId,DateTime transactionDate, ICollection<InventoryTransactionItem> items) : base(storageLocationId, transactionDate, items)
    {
    }

    private AdjustmentTransaction() { }

    public override string GetTransactionType() => "Adjustment";
}
