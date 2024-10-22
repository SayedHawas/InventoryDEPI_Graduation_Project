using Microsoft.AspNetCore.Mvc;
using smERP.Application.Features.Auth.Commands.Models;
using smERP.Application.Features.Auth.Commands.Results;
using smERP.Application.Features.Auth.Queries.Models;
using smERP.SharedKernel.Responses;

namespace smERP.WebApi.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthController : AppControllerBase
{
    [HttpPost("Register")]
    public async Task<IActionResult> Register(RegisterCommandModel<IResult<RegisterResult>> request)
    {
        var response = await Mediator.Send(request);
        var apiResult = response.ToApiResult();
        return StatusCode(apiResult.StatusCode, apiResult);
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login(LoginCommandModel<IResult<LoginResult>> request)
    {
        var response = await Mediator.Send(request);
        var apiResult = response.ToApiResult();
        SetRefreshTokenInCookie(apiResult.Value.RefreshToken, apiResult.Value.RefreshTokenExpirationDate);
        return StatusCode(apiResult.StatusCode, apiResult);
    }

    [HttpPost("Roles")]
    public async Task<IActionResult> CreateRole(string roleName)
    {
        var response = await Mediator.Send(new AddRoleCommandModel(roleName));
        var apiResult = response.ToApiResult();
        return StatusCode(apiResult.StatusCode, apiResult);
    }

    [HttpGet("Roles")]
    public async Task<IActionResult> GetAllRoles()
    {
        var response = await Mediator.Send(new GetAllRolesQuery());
        var apiResult = response.ToApiResult();
        return StatusCode(apiResult.StatusCode, apiResult);
    }

    [HttpGet("Users")]
    public async Task<IActionResult> GetPaginatedUsers([FromQuery] GetPaginatedUsersQuery request)
    {
        var response = await Mediator.Send(request);
        var apiResult = response.ToApiResult();
        return StatusCode(apiResult.StatusCode, apiResult);
    }

    [HttpGet("Users/{userId}")]
    public async Task<IActionResult> GetUser(string userId)
    {
        var response = await Mediator.Send(new GetUserQuery(userId));
        var apiResult = response.ToApiResult();
        return StatusCode(apiResult.StatusCode, apiResult);
    }

    [HttpPut("Users/{userId}")]
    public async Task<IActionResult> UpdateUser(string userId,EditUserCommandModel request)
    {
        var command = EditUserCommandModel.CreateEditUserCommand(
            userId,
            request.BranchId,
            request.FirstName,
            request.LastName,
            request.Email,
            request.Password,
            request.PhoneNumber,
            request.Address,
            request.Roles
        );
        var response = await Mediator.Send(command);
        var apiResult = response.ToApiResult();
        return StatusCode(apiResult.StatusCode, apiResult);
    }

    [HttpPut("Users/Disable")]
    public async Task<IActionResult> DisableUserAccount(GetPaginatedUsersQuery request)
    {
        var response = await Mediator.Send(request);
        var apiResult = response.ToApiResult();
        return StatusCode(apiResult.StatusCode, apiResult);
    }

    [HttpPut("Users/Enable")]
    public async Task<IActionResult> EnableUserAccount(GetPaginatedUsersQuery request)
    {
        var response = await Mediator.Send(request);
        var apiResult = response.ToApiResult();
        return StatusCode(apiResult.StatusCode, apiResult);
    }

    private void SetRefreshTokenInCookie(string refreshToken, DateTime expires)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = expires.ToLocalTime(),
            Secure = true,
            IsEssential = true,
            SameSite = SameSiteMode.None
        };

        Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
    }
}
