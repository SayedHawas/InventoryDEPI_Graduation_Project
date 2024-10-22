using MediatR;
using smERP.Application.Features.Suppliers.Commands.Models;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.Auth.Commands.Models;

public record EditUserCommandModel(
    string? UserId,
    int? BranchId,
    string? FirstName,
    string? LastName,
    string? Email,
    string? Password,
    string? PhoneNumber,
    Address? Address,
    List<string>? Roles
) : IRequest<IResultBase>
{
    public static EditUserCommandModel CreateEditUserCommand(
    string userId,
    int? branchId,
    string? firstName,
    string? lastName,
    string? email,
    string? password,
    string? phoneNumber,
    Address? address,
    List<string>? roles)
    {
        return new EditUserCommandModel(userId, branchId, firstName, lastName, email, password, phoneNumber, address, roles);
    }

};

