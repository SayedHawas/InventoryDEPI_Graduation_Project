using smERP.SharedKernel.Responses;
using MediatR;

namespace smERP.Application.Features.Auth.Commands.Models;
public record EditRoleCommandModel(string RoleName, string NewRoleName) : IRequest<IResultBase>;