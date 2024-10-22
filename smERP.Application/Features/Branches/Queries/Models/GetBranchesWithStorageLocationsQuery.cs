using MediatR;
using smERP.Application.Features.Branches.Queries.Responses;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.Branches.Queries.Models;

public record GetBranchesWithStorageLocationsQuery() : IRequest<IResult<IEnumerable<GetBranchesWithStorageLocationsQueryResponse>>>;