namespace smERP.Domain.Entities.Product;

public class ProductInstanceItem : Entity
{
    public int StorageLocationId { get; private set; }
    public int ProductInstanceId { get; private set; }
    public string SerialNumber { get; private set; } = null!;
    public string Status { get; private set; } = null!;
    public DateOnly? ExpirationDate { get; private set; }

    private ProductInstanceItem() { }

    internal ProductInstanceItem(int storageLocationId, int productInstanceId, string serialNumber, string status)
    {
        StorageLocationId = storageLocationId;
        ProductInstanceId = productInstanceId;
        SerialNumber = serialNumber;
        Status = status;
        ExpirationDate = null;
    }

    internal ProductInstanceItem(int storageLocationId, int productInstanceId, string serialNumber, string status, DateOnly expirationDate)
    {
        StorageLocationId = storageLocationId;
        ProductInstanceId = productInstanceId;
        SerialNumber = serialNumber;
        Status = status;
        ExpirationDate = expirationDate;
    }

    internal void UpdateStatus(string status)
    {
        Status = status;
    }
}