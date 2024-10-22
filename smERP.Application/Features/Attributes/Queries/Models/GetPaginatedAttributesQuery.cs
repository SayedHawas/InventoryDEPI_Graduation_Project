using MediatR;
using smERP.Application.Features.Attributes.Queries.Responses;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.Attributes.Queries.Models;

public record GetPaginatedAttributesQuery() : PaginationParameters, IRequest<IResult<PagedResult<GetAttributesQueryResponse>>>;
