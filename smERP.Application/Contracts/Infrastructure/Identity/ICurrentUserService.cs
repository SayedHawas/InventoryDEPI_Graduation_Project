using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace smERP.Application.Contracts.Infrastructure.Identity;

public interface ICurrentUserService
{
    ClaimsPrincipal User { get; }
    int? GetBranchId();
    int? GetUserId();
    string? GetUserName();
}