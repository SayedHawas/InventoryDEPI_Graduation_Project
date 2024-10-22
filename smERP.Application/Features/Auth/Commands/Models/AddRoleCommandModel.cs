using MediatR;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.Auth.Commands.Models;

public record AddRoleCommandModel(string RoleName) : IRequest<IResultBase>;
