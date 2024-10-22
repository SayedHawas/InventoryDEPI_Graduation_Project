using smERP.Application.Features.Branches.Queries.Models;
using smERP.Application.Features.Suppliers.Commands.Models;

namespace smERP.Application.Features.Auth.Queries.Responses;

public record GetUserQueryResponse(
    string UserId,
    string FirstName,
    string LastName,
    string UserName,
    string Email,
    string PhoneNumber,
    Address Address,
    int BranchId,
    IEnumerable<string> Roles
    );