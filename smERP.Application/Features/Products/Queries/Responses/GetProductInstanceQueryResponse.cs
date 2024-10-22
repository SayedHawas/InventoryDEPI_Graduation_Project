using smERP.Application.Features.Attributes.Queries.Responses;

namespace smERP.Application.Features.Products.Queries.Responses;

public record GetProductInstanceQueryResponse(int ProductId, int ProductInstanceId, string Sku, int QuantityInStock, decimal BuyingPrice, decimal SellingPrice, string? Image, IEnumerable<ProductInstanceAttribute> Attributes);

public record ProductInstanceAttribute(int AttributeId, int AttributeValueId);