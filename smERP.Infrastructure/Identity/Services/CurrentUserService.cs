using Microsoft.AspNetCore.Http;
using smERP.Application.Contracts.Infrastructure.Identity;
using System.Security.Claims;

namespace smERP.Infrastructure.Identity.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public ClaimsPrincipal User => _httpContextAccessor.HttpContext.User;

    public int? GetBranchId()
    {
        var branchIdClaim = User?.FindFirst("branch");
        return branchIdClaim != null ? int.Parse(branchIdClaim.Value) : null;
    }

    public int? GetUserId()
    {
        throw new NotImplementedException();
    }

    public string? GetUserName()
    {
        throw new NotImplementedException();
    }
}