using MediatR;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.Auth.Queries.Models;

public class GetUserRolesQuery : IRequest<IResult<IEnumerable<string>>>
{
    public string UserId { get; set; }
}
