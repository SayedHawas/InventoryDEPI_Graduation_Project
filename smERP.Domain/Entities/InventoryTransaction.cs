namespace smERP.Domain.Entities;

//public abstract class InventoryTransaction : Entity
//{
//    public DateTime TransactionDate { get; protected set; }
//    public Guid UserId { get; protected set; }
//    public List<InventoryTransactionItem> Items { get; protected set; } = new List<InventoryTransactionItem>();

//    protected InventoryTransaction() { }

//    protected InventoryTransaction(Guid userId)
//    {
//        TransactionDate = DateTime.UtcNow;
//        UserId = userId;
//    }

//    public void AddItem(Guid inventoryItemId, int quantity)
//    {
//        Items.Add(new InventoryTransactionItem(inventoryItemId, quantity));
//    }
//}
