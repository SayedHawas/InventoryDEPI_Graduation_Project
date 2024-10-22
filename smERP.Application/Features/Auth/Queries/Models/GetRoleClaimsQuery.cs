using MediatR;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.Auth.Queries.Models;

public record GetRoleClaimsQuery(string RoleName) : IRequest<IResult<IEnumerable<string>>>;
