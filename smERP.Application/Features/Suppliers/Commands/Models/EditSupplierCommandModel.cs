using MediatR;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.Suppliers.Commands.Models;

public record EditSupplierCommandModel(
    int SupplierId,
    string? EnglishName,
    string? ArabicName,
    List<Address>? Addresses) : IRequest<IResultBase>
{
    public int SupplierId { get; set; } = SupplierId;
}

