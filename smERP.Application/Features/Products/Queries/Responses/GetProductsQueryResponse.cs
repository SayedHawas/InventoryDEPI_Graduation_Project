namespace smERP.Application.Features.Products.Queries.Responses;

public record GetProductsQueryResponse(int ProductId, int ProductInstanceId, string Name, int? ShelfLifeInDays, bool IsWarranted);