using MediatR;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.Auth.Commands.Models;

public record DisableUserAccountCommandModel(string UserId) : IRequest<IResultBase>;