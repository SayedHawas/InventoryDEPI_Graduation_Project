using MediatR;
using smERP.Application.Features.Branches.Queries.Responses;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.Branches.Queries.Models;

public record GetPaginatedBranchesQuery() : PaginationParameters, IRequest<IResult<PagedResult<GetPaginatedBranchesQueryResponse>>>;