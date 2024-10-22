using MediatR;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.Auth.Queries.Models;

public class GetAllRolesQuery : IRequest<IResult<IEnumerable<string?>>>
{
}
