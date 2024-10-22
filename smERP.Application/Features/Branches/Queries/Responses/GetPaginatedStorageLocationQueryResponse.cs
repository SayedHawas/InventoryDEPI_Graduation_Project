namespace smERP.Application.Features.Branches.Queries.Responses;

public record GetPaginatedStorageLocationsQueryResponse(
    int StorageLocationId,
    string Name,
    IEnumerable<ProductStored> Products);

public record ProductStored(
    int ProductInstanceId,
    string Sku,
    string Name,
    int Quantity,
    decimal BuyingPrice,
    decimal SellingPrice);