using MediatR;
using smERP.Application.Features.Branches.Queries.Responses;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.Branches.Queries.Models;

public record GetBranchQuery(int BranchId) : IRequest<IResult<GetBranchQueryResponse>>;