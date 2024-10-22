using MediatR;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.Auth.Commands.Models;

public record AssignRoleToUserCommandModel(string UserId, string RoleName) : IRequest<IResultBase>;
