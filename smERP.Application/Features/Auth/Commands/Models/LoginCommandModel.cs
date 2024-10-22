using MediatR;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.Auth.Commands.Models;

public record LoginCommandModel<TResult>(string Email, string Password) : IRequest<TResult> where TResult : IResultBase;