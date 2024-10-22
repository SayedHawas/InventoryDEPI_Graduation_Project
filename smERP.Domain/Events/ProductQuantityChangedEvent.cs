namespace smERP.Domain.Events;

public record ProductsQuantityChangedEvent(
    int TransactionId,
    int StorageLocationId,
    string TransactionType,
    List<(int ProductIsntanceId, int EffectedQuantity, bool IsTracked, int? ShelfLifeInDays, List<(string SerialNumber, string Status, DateOnly? ExpirationDate)>? SerialNumbers)> ProductEntries) : IDomainEvent;