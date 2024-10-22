using MediatR;
using smERP.Application.Contracts.Infrastructure.Identity;
using smERP.Application.Features.Auth.Queries.Models;
using smERP.Application.Features.Auth.Queries.Responses;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.Auth.Queries.Handlers;

public class AuthQueryHandler(IAuthService authService) :
    IRequestHandler<GetAllRolesQuery, IResult<IEnumerable<string?>>>,
    IRequestHandler<GetPaginatedUsersQuery, IResult<PagedResult<GetPaginatedUsersQueryResponse>>>,
    IRequestHandler<GetUserQuery, IResult<GetUserQueryResponse>>

{
    private readonly IAuthService _authService = authService;

    public async Task<IResult<IEnumerable<string?>>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
    {
        var roles = await _authService.GetAllRoles();
        if (roles == null) return new Result<IEnumerable<string?>>();
        return roles;
    }

    public async Task<IResult<PagedResult<GetPaginatedUsersQueryResponse>>> Handle(GetPaginatedUsersQuery request, CancellationToken cancellationToken)
    {
        return await _authService.GetPaginatedUsers(request);
    }

    public async Task<IResult<GetUserQueryResponse>> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        return await _authService.GetUserById(request.userId);
    }
}
