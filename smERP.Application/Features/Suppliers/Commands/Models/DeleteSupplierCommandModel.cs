using MediatR;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.Suppliers.Commands.Models;

public record DeleteSupplierCommandModel(int SupplierId) : IRequest<IResultBase>;
