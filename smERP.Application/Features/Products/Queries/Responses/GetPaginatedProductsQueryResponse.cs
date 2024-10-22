using smERP.Domain.Entities.Product;

namespace smERP.Application.Features.Products.Queries.Responses;

public record GetPaginatedProductsQueryResponse(
    int ProductId,
    string ModelNumber,
    string Name,
    bool IsTracked,
    int? ShelfLifeInDays,
    int? WarrantyInDays,
    string Brand,
    string Category,
    IEnumerable<ProductInstance> Instances
    );

public record ProductInstance(
    int ProductInstanceId,
    string Sku,
    int QuantityInStock,
    decimal BuyingPrice,
    decimal SellingPrice,
    string Image
    );