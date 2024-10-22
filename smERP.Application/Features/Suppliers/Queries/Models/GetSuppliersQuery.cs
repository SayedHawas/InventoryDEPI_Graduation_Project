using MediatR;
using smERP.Application.Features.Branches.Queries.Models;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.Suppliers.Queries.Models;

public record GetSuppliersQuery() : IRequest<IResult<IEnumerable<SelectOption>>>;