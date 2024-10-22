namespace smERP.Application.Features.Brands.Queries.Responses;

public record GetPaginatedBrandsQueryResponse(
    int BrandId,
    string Name,
    int ProductCount);