using MediatR;
using smERP.Application.Features.Categories.Queries.Responses;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.Categories.Queries.Models;

public record GetPaginatedCategoriesQuery() : PaginationParameters, IRequest<IResult<PagedResult<GetPaginatedCategoriesQueryResponse>>>;