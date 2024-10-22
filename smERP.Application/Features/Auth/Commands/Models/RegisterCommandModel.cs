using MediatR;
using smERP.Application.Features.Suppliers.Commands.Models;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.Auth.Commands.Models;

public record RegisterCommandModel<TResult>(
    int BranchId,
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string PhoneNumber,
    Address Address,
    List<string> Roles
) : IRequest<TResult>
    where TResult : IResultBase;