namespace smERP.Domain.Entities.InventoryTransaction;

public class InventoryTransactionItemUnit : Entity
{
    public int TransactionItemId { get; private set; }
    public int TransactionId { get; private set; }
    public int ProductInstanceId { get; private set; }
    public string SerialNumber { get; private set; } = null!;
    public string Status { get; private set; } = "Available";

    internal InventoryTransactionItemUnit(int productInstanceId, string serialNumber)
    {
        ProductInstanceId = productInstanceId;
        SerialNumber = serialNumber;
    }
    internal InventoryTransactionItemUnit(int transactionId, int productInstanceId, string serialNumber)
    {
        TransactionId = transactionId;
        ProductInstanceId = productInstanceId;
        SerialNumber = serialNumber;
    }
}
