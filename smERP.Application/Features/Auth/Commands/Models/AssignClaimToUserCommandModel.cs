using MediatR;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.Auth.Commands.Models;

public record AssignClaimToUserCommandModel(string UserId, string ClaimType, string ClaimValue) : IRequest<IResultBase>;
