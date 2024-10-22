using MediatR;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.Auth.Queries.Models;

public class GetAllClaimsQuery : IRequest<IResult<IEnumerable<string>>>
{
}
