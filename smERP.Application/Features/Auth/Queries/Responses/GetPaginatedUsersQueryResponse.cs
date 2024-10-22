namespace smERP.Application.Features.Auth.Queries.Responses;

public record GetPaginatedUsersQueryResponse(
    string UserId,
    string FirstName,
    string LastName,
    string UserName,
    string Email,
    string PhoneNumber,
    string Address,
    bool IsAccountDisabled,
    int BranchId,
    List<string> Roles
    );