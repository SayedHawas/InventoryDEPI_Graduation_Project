using Azure.Core;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using smERP.Application.Contracts.Infrastructure.Identity;
using smERP.Application.Features.Auth.Commands.Models;
using smERP.Application.Features.Auth.Commands.Results;
using smERP.Application.Features.Auth.Queries.Responses;
using smERP.Application.Features.Branches.Queries.Models;
using smERP.Domain.Entities.Organization;
using smERP.Domain.ValueObjects;
using smERP.Infrastructure.Identity.Models;
using smERP.Infrastructure.Identity.Models.Users;
using smERP.SharedKernel.Localizations.Extensions;
using smERP.SharedKernel.Localizations.Resources;
using smERP.SharedKernel.Responses;
using System.Data;
using System.Diagnostics.Metrics;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace smERP.Infrastructure.Identity.Services;

public class AuthService(
    SignInManager<ApplicationUser> signInManager,
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager,
    IOptions<JwtSettings> jwtSettings,
    ICurrentUserService currentUserService,
    IAuthorizationService authorizationService) : IAuthService
{
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;
    private readonly ICurrentUserService _currentUserService = currentUserService;
    private readonly IAuthorizationService _authorizationService = authorizationService;

    public async Task<IResult<LoginResult>> Login(LoginCommandModel<IResult<LoginResult>> request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            if (user != null) await _userManager.AccessFailedAsync(user);

            return new Result<LoginResult>()
                .WithError(SharedResourcesKeys.PasswordOrEmailNotCorrect.Localize())
                .WithStatusCode(HttpStatusCode.BadRequest);
        }

        if (user.IsAccountDisabled)
            return new Result<LoginResult>()
                .WithBadRequestResult(SharedResourcesKeys.AccountDisabledContactAdmin.Localize());

        JwtSecurityToken jwtSecurityToken = await GenerateToken(user);

        if (!user.IsExistingRefreshTokenValid())
        {
            user.GenerateRefreshToken();
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenString = tokenHandler.WriteToken(jwtSecurityToken);

        var loginResult = new LoginResult
        (
            tokenString,
            user.RefreshToken,
            user.RefreshTokenExpiration
        );

        return new Result<LoginResult>(loginResult);
    }

    public async Task<IResult<RegisterResult>> Register(RegisterCommandModel<IResult<RegisterResult>> request)
    {
        var existingUser = await _userManager.FindByNameAsync(request.FirstName + request.LastName);

        if (existingUser != null)
            return new Result<RegisterResult>()
                .WithError(SharedResourcesKeys.DoesExist.Localize(SharedResourcesKeys.UserName))
                .WithStatusCode(HttpStatusCode.BadRequest);

        var existingEmail = await _userManager.FindByEmailAsync(request.Email);
        if (existingEmail != null)
            return new Result<RegisterResult>()
                .WithError(SharedResourcesKeys.DoesExist.Localize(SharedResourcesKeys.Email))
                .WithStatusCode(HttpStatusCode.BadRequest);

        var existingRoles = await _roleManager.Roles.Where(role => request.Roles.Contains(role.Name)).ToListAsync();
        if (existingRoles.Count != request.Roles.Count)
            return new Result<RegisterResult>()
                .WithError(SharedResourcesKeys.SomeItemsIn___ListAreNotCorrect.Localize(SharedResourcesKeys.Role))
                .WithStatusCode(HttpStatusCode.BadRequest);

        var user = new ApplicationUser
        {
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            UserName = request.FirstName + "_" + request.LastName,
            PhoneNumber = request.PhoneNumber,
            EmailConfirmed = true,
        };

        user.AddBranch(request.BranchId);

        user.Employee.AddAddress(request.Address.Country, request.Address.City, request.Address.State, request.Address.Street, request.Address.PostalCode, request.Address.Comment);

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
            return new Result<RegisterResult>()
                .WithErrors(result.Errors.Select(a => a.Description).ToList())
                .WithStatusCode(HttpStatusCode.BadRequest);

        await _userManager.AddToRolesAsync(user, request.Roles);

        return new Result<RegisterResult>(new RegisterResult() { UserId = user.Id });
    }

    public async Task<IResultBase> DisableUserAccount(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            new Result<bool>()
                .WithBadRequestResult(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.User.Localize()));

        user.IsAccountDisabled = true;

        await _userManager.UpdateAsync(user);

        return new Result<bool>();
    }

    public async Task<IResultBase> EnableUserAccount(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            new Result<bool>()
                .WithBadRequestResult(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.User.Localize()));

        user.IsAccountDisabled = false;

        await _userManager.UpdateAsync(user);

        return new Result<bool>();
    }

    public async Task<IResultBase> CreateRole(AddRoleCommandModel request)
    {
        var doesRoleExist = await _roleManager.RoleExistsAsync(request.RoleName);
        if (doesRoleExist)
            return new Result<string>()
                .WithBadRequest(SharedResourcesKeys.DoesExist.Localize(SharedResourcesKeys.Role.Localize()));

        var roleToBeCreated = new IdentityRole(request.RoleName);
        var roleToBeCreatedResult = await _roleManager.CreateAsync(roleToBeCreated);

        if (!roleToBeCreatedResult.Succeeded)
            return new Result<string>()
                .WithBadRequest(roleToBeCreatedResult.Errors.Select(x => x.Description));

        return new Result<string>();
    }

    public async Task<IResultBase> EditRole(EditRoleCommandModel request)
    {
        var roleToBeEdited = await _roleManager.FindByNameAsync(request.RoleName);
        if (roleToBeEdited == null)
            return new Result<string>()
                .WithBadRequest(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.Role.Localize()));

        roleToBeEdited.Name = request.NewRoleName;

        var roleToBeEditedResult = await _roleManager.UpdateAsync(roleToBeEdited);
        if (!roleToBeEditedResult.Succeeded)
            return new Result<string>()
                .WithBadRequest(roleToBeEditedResult.Errors.Select(x => x.Description));

        return new Result<string>();
    }

    public async Task<IResultBase> DeleteRole(DeleteRoleCommandModel request)
    {
        var roleToBeDeleted = await _roleManager.FindByNameAsync(request.RoleName);
        if (roleToBeDeleted == null)
            return new Result<string>()
                .WithBadRequest(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.Role.Localize()));

        var roleToBeDeletedResult = await _roleManager.DeleteAsync(roleToBeDeleted);
        if (!roleToBeDeletedResult.Succeeded)
            return new Result<string>()
                .WithBadRequest(roleToBeDeletedResult.Errors.Select(x => x.Description));

        return new Result<string>();
    }

    public async Task<IResultBase> AddClaimToRole(AddClaimToRoleCommandModel request)
    {
        var role = await _roleManager.FindByNameAsync(request.RoleName);
        if (role == null)
            return new Result<string>()
                .WithBadRequest(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.Role.Localize()));

        var roleClaims = await _roleManager.GetClaimsAsync(role);

        var doesClaimRoleExist = roleClaims.Any(x => x.ValueType == request.ClaimType && x.Value == request.ClaimValue);
        if (doesClaimRoleExist)
            return new Result<string>()
                .WithBadRequest(SharedResourcesKeys.DoesExist.Localize(SharedResourcesKeys.Claim.Localize()));

        var claimRoleToBeCreated = new Claim(request.ClaimType, request.ClaimValue);

        var claimRoleToBeCreatedResult = await _roleManager.AddClaimAsync(role, claimRoleToBeCreated);
        if (!claimRoleToBeCreatedResult.Succeeded)
            return new Result<string>()
                .WithBadRequest(claimRoleToBeCreatedResult.Errors.Select(x => x.Description));

        return new Result<string>();
    }

    public async Task<IResultBase> AddClaimToUser(AssignClaimToUserCommandModel request)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null)
            return new Result<string>()
                .WithBadRequest(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.User.Localize()));

        var existingUserClaims = await _userManager.GetClaimsAsync(user);

        var doesUserClaimExist = existingUserClaims.Any(x => x.ValueType == request.ClaimType && x.Value == request.ClaimValue);
        if (doesUserClaimExist)
            return new Result<string>()
                .WithBadRequest(SharedResourcesKeys.DoesExist.Localize(SharedResourcesKeys.User.Localize()));

        var userClaimToBeCreated = new Claim(request.ClaimType, request.ClaimValue);

        var userClaimToBeCreatedResult = await _userManager.AddClaimAsync(user, userClaimToBeCreated);
        if (!userClaimToBeCreatedResult.Succeeded)
            return new Result<string>()
                  .WithBadRequest(userClaimToBeCreatedResult.Errors.Select(x => x.Description));

        return new Result<string>();
    }

    public async Task<IResultBase> AddRoleToUser(AssignRoleToUserCommandModel request)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null)
            return new Result<string>()
                .WithBadRequest(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.User.Localize()));

        var role = await _roleManager.FindByNameAsync(request.RoleName);
        if (role == null)
            return new Result<string>()
                .WithBadRequest(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.Role.Localize()));

        var userRoles = await _userManager.GetRolesAsync(user);

        var doesUserRoleExist = userRoles.Any(x => x == request.RoleName);
        if (doesUserRoleExist)
            return new Result<string>()
                .WithBadRequest(SharedResourcesKeys.DoesExist.Localize(SharedResourcesKeys.Role.Localize()));

        var userRoleCreateResult = await _userManager.AddToRoleAsync(user, request.RoleName);
        if (!userRoleCreateResult.Succeeded)
            return new Result<string>()
                .WithBadRequest(userRoleCreateResult.Errors.Select(x => x.Description));

        return new Result<string>();
    }

    public async Task<IResult<IEnumerable<string?>>> GetAllRoles()
    {
        var roles = await _roleManager.Roles.Select(x => x.Name).ToListAsync();
        if (roles == null) return new Result<IEnumerable<string?>>();
        return new Result<IEnumerable<string?>>(roles);
    }

    public async Task<IResult<IEnumerable<(string Type, string Value)>>> GetRoleClaims(string roleName)
    {
        var role = await _roleManager.FindByNameAsync(roleName);
        if (role == null)
            return new Result<IEnumerable<(string, string)>>()
                .WithBadRequestResult(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.Role.Localize()));

        var roleClaims = await _roleManager.GetClaimsAsync(role);

        return new Result<IEnumerable<(string, string)>>(roleClaims.Select(x => (x.Type, x.Value)));
    }

    public async Task<IResult<IEnumerable<(string Type, string Value)>>> GetUserClaims(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return new Result<IEnumerable<(string, string)>>()
                .WithBadRequestResult(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.User.Localize()));

        var userClaims = await _userManager.GetClaimsAsync(user);

        return new Result<IEnumerable<(string, string)>>(userClaims.Select(x => (x.Type, x.Value)));
    }

    public async Task<IResult<IEnumerable<string>>> GetUserRoles(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return new Result<IEnumerable<string>>()
                .WithBadRequestResult(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.User.Localize()));

        var userRoles = await _userManager.GetRolesAsync(user);

        return new Result<IEnumerable<string>>(userRoles);
    }

    //public async Task<BaseResponse<string>> SendResetPasswordCode(string email)
    //{
    //    var trans = await _identityDbContext.Database.BeginTransactionAsync();

    //    try
    //    {
    //        var user = await _userManager.FindByEmailAsync(email);

    //        if (user == null)
    //            return NotFound<string>(_localizer["UserNotFound"]);

    //        var chars = "0123456789";
    //        var random = new Random();
    //        var randomNumber = new string(Enumerable.Repeat(chars, 6).Select(s => s[random.Next(s.Length)]).ToArray());

    //        user.Code = randomNumber;

    //        var updateResult = await _userManager.UpdateAsync(user);

    //        if (!updateResult.Succeeded)
    //            return BadRequest<string>(_localizer["ErrorInUpdateUser"]);

    //        var message = $"Code To Reset Password: {user.Code}";

    //        await _emailsService.SendEmail(user.Email, message, "Reset Password");

    //        await trans.CommitAsync();

    //        return Success<string>(_localizer["Success"]);
    //    }
    //    catch (Exception ex)
    //    {
    //        await trans.RollbackAsync();
    //        return BadRequest<string>(_localizer["Failed"]);
    //    }
    //}

    //public async Task<BaseResponse<string>> ConfirmAndResetPassword(string code, string email, string newPassword)
    //{
    //    using (var trans = await _identityDbContext.Database.BeginTransactionAsync())
    //    {
    //        try
    //        {
    //            var user = await _userManager.FindByEmailAsync(email);

    //            if (user == null)
    //                return NotFound<string>(_localizer["UserNotFound"]);

    //            var userCode = user.Code;

    //            if (userCode == code)
    //            {
    //                // Code is valid, proceed to reset the password
    //                await _userManager.RemovePasswordAsync(user);
    //                await _userManager.AddPasswordAsync(user, newPassword);

    //                await trans.CommitAsync();

    //                return Success<string>(_localizer["PasswordResetSuccess"]);
    //            }

    //            return BadRequest<string>(_localizer["InvalidCode"]);
    //        }
    //        catch (Exception ex)
    //        {
    //            await trans.RollbackAsync();
    //            return BadRequest<string>(_localizer["Failed"]);
    //        }
    //    }
    //}

    private async Task<JwtSecurityToken> GenerateToken(ApplicationUser user)
    {
        var userClaims = await _userManager.GetClaimsAsync(user);
        var roles = await _userManager.GetRolesAsync(user);

        var roleClaims = new List<Claim>();

        for (int i = 0; i < roles.Count; i++)
        {
            roleClaims.Add(new Claim("roles", roles[i]));
        }

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
            new Claim("firstName", user.FirstName),
            new Claim("lastName", user.LastName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("uid", user.Id),
            new Claim("branch", user.Employee.BranchId.ToString())
        }
        .Union(userClaims)
        .Union(roleClaims);

        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var jwtSecurityToken = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
            signingCredentials: signingCredentials);
        return jwtSecurityToken;
    }

    public async Task<IResult<PagedResult<GetPaginatedUsersQueryResponse>>> GetPaginatedUsers(PaginationParameters parameters)
    {
        var query = _userManager.Users.AsQueryable();

        query = await ApplyRoleBasedFilter(query);

        if (!string.IsNullOrEmpty(parameters.SearchTerm))
        {
            query = query.Where(u => u.UserName.Contains(parameters.SearchTerm) || u.Email.Contains(parameters.SearchTerm));
        }

        if (!string.IsNullOrEmpty(parameters.SortBy))
        {
            query = parameters.SortDescending
                ? query.OrderByDescending(u => EF.Property<object>(u, parameters.SortBy))
                : query.OrderBy(u => EF.Property<object>(u, parameters.SortBy));
        }

        var totalCount = await query.CountAsync();

        var pagedUsers = await query
            .AsNoTracking()
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .Select(u => new
            {
                u.Employee,
                u.Id,
                u.FirstName,
                u.LastName,
                u.UserName,
                u.Email,
                u.Employee.BranchId,
                u.PhoneNumber,
                u.Employee.Address,
                u.IsAccountDisabled
            })
            .ToListAsync();

        var userRoles = new Dictionary<string, List<string>>();
        foreach (var user in pagedUsers)
        {
            var roles = await _userManager.GetRolesAsync(await _userManager.FindByIdAsync(user.Id));
            userRoles[user.Id] = roles.ToList();
        }

        var pagedData = pagedUsers.Select(u => new GetPaginatedUsersQueryResponse(u.Id, u.FirstName, u.LastName, u.UserName, u.Email, u.PhoneNumber, u.Employee.Address?.ToSingleLineString() ?? "Not Available", u.IsAccountDisabled, u.BranchId, userRoles[u.Id])).ToList();

        var pagedResult = new PagedResult<GetPaginatedUsersQueryResponse>()
        {
            TotalCount = totalCount,
            PageNumber = parameters.PageNumber,
            PageSize = parameters.PageSize,
            Data = pagedData,
        };

        return new Result<PagedResult<GetPaginatedUsersQueryResponse>>(pagedResult);
    }

    public async Task<IResult<GetUserQueryResponse>> GetUserById(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return new Result<GetUserQueryResponse>()
                .WithBadRequestResult(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.User.Localize()));

        var userRoles = await _userManager.GetRolesAsync(user);

        var response = new GetUserQueryResponse(
            user.Id,
            user.FirstName,
            user.LastName,
            user.UserName ?? "",
            user.Email ?? "",
            user.PhoneNumber ?? "",
            new Application.Features.Suppliers.Commands.Models.Address(
                user.Employee.Address?.Country ?? "",
                user.Employee.Address?.City ?? "",
                user.Employee.Address?.State ?? "",
                user.Employee.Address?.Street ?? "",
                user.Employee.Address?.PostalCode ?? "",
                user.Employee.Address?.Comment),
            user.Employee.BranchId,
            userRoles);

        return new Result<GetUserQueryResponse>(response);
    }

    public async Task<IResultBase> UpdateUser(EditUserCommandModel request)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null)
            return new Result<bool>()
                .WithBadRequestResult(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.User.Localize()));

        if (request.FirstName != null)
            user.FirstName = request.FirstName;

        if (request.LastName != null)
            user.LastName = request.LastName;

        if (request.Email != null)
            user.Email = request.Email;

        if (request.BranchId.HasValue)
            user.Employee.BranchId = request.BranchId.Value;

        if (request.PhoneNumber != null)
            user.PhoneNumber = request.PhoneNumber;

        if (request.Address != null)
            user.Employee.AddAddress(request.Address.Country, request.Address.City, request.Address.State, request.Address.Street, request.Address.PostalCode, request.Address.Comment);

        if (!string.IsNullOrEmpty(request.Password))
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, request.Password);
            if (!result.Succeeded)
                return new Result<bool>().WithBadRequestResult(result.Errors.Select(e => e.Description));
        }

        if (request.Roles != null && request.Roles.Count > 0)
        {
            var currentRoles = await _userManager.GetRolesAsync(user);
            var rolesToRemove = currentRoles.Except(request.Roles);
            var rolesToAdd = request.Roles.Except(currentRoles);

            if (rolesToRemove.Any())
                await _userManager.RemoveFromRolesAsync(user, rolesToRemove);

            if (rolesToAdd.Any())
                await _userManager.AddToRolesAsync(user, rolesToAdd);
        }

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
            return new Result<bool>().WithBadRequestResult(updateResult.Errors.Select(e => e.Description));

        return new Result<bool>();
    }

    private async Task<IQueryable<ApplicationUser>> ApplyRoleBasedFilter(IQueryable<ApplicationUser> query)
    {
        var user = _currentUserService.User;

        if ((await _authorizationService.AuthorizeAsync(user, null, "AdminPolicy")).Succeeded)
        {
            return query;
        }
        else if ((await _authorizationService.AuthorizeAsync(user, null, "BranchManagerPolicy")).Succeeded)
        {
            var branchId = _currentUserService.GetBranchId();
            return query.Where(u => u.Employee.BranchId == branchId);
        }

        return query.Where(e => false);
    }
}
