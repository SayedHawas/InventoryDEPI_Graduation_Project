
namespace smERP.Domain.Entities.InventoryTransaction;

public class SalesTransaction : ExternalEntityInventoryTransaction
{
    public int ClientId { get; private set; }
    public SalesTransaction(int storageLocationId, DateTime transactionDate, ICollection<TransactionPayment> payments, ICollection<InventoryTransactionItem> items) : base(storageLocationId, transactionDate, payments, items)
    {
    }
    
    private SalesTransaction() { }

    public override string GetTransactionType() => "Sales";
}
