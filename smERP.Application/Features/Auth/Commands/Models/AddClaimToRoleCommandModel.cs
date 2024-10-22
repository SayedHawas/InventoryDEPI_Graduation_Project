using MediatR;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.Auth.Commands.Models;

public record AddClaimToRoleCommandModel(string RoleName, string ClaimType, string ClaimValue) : IRequest<IResultBase>;
