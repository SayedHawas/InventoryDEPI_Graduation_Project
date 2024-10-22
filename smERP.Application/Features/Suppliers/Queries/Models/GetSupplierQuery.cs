using MediatR;
using smERP.Application.Features.Suppliers.Queries.Responses;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.Suppliers.Queries.Models;

public record GetSupplierQuery(int SupplierId) : IRequest<IResult<GetSupplierQueryResponse>>;
