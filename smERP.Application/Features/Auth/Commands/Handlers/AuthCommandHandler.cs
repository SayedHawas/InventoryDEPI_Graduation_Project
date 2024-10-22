using MediatR;
using Microsoft.AspNetCore.Http;
using smERP.Application.Contracts.Infrastructure.Identity;
using smERP.Application.Features.Auth.Commands.Models;
using smERP.Application.Features.Auth.Commands.Results;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.Auth.Commands.Handlers;

public class AuthCommandHandler(IAuthService authService) :
    IRequestHandler<RegisterCommandModel<IResult<RegisterResult>>, IResult<RegisterResult>>,
    IRequestHandler<LoginCommandModel<IResult<LoginResult>>, IResult<LoginResult>>,
    IRequestHandler<AddRoleCommandModel, IResultBase>,
    IRequestHandler<DisableUserAccountCommandModel, IResultBase>,
    IRequestHandler<EnableUserAccountCommandModel, IResultBase>,
    IRequestHandler<EditUserCommandModel, IResultBase>

{
    private readonly IAuthService _authService = authService;

    public async Task<IResult<RegisterResult>> Handle(RegisterCommandModel<IResult<RegisterResult>> request, CancellationToken cancellationToken)
    {
        return await _authService.Register(request);
    }

    public async Task<IResult<LoginResult>> Handle(LoginCommandModel<IResult<LoginResult>> request, CancellationToken cancellationToken)
    {
        return await _authService.Login(request);
    }

    public async Task<IResultBase> Handle(AddRoleCommandModel request, CancellationToken cancellationToken)
    {
        return await _authService.CreateRole(request);
    }

    public async Task<IResultBase> Handle(DisableUserAccountCommandModel request, CancellationToken cancellationToken)
    {
        return await _authService.DisableUserAccount(request.UserId);
    }

    public async Task<IResultBase> Handle(EnableUserAccountCommandModel request, CancellationToken cancellationToken)
    {
        return await _authService.EnableUserAccount(request.UserId);
    }

    public async Task<IResultBase> Handle(EditUserCommandModel request, CancellationToken cancellationToken)
    {
        return await _authService.UpdateUser(request);
    }
}