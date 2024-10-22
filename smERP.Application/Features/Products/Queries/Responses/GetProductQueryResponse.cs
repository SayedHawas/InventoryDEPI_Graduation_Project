using smERP.Application.Features.Branches.Queries.Models;

namespace smERP.Application.Features.Products.Queries.Responses;

public record GetProductQueryResponse(
    int ProductId,
    string EnglishName,
    string ArabicName,
    string ModelName,
    string Description,
    int? ShelfLifeInDays,
    int? WarrantyInDays,
    int BrandId,
    int CategoryId,
    IEnumerable<GetProductInstance> Instances
    );

public record GetProductInstance(
    int ProductInstanceId,
    string Sku,
    int QuantityInStock,
    decimal BuyingPrice,
    decimal SellingPrice,
    string Image
    );