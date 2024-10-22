using MediatR;
using smERP.Application.Features.Brands.Queries.Responses;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.Brands.Queries.Models;

public record GetPaginatedBrandsQuery() : PaginationParameters, IRequest<IResult<PagedResult<GetPaginatedBrandsQueryResponse>>>;