using MediatR;
using smERP.Application.Features.Branches.Queries.Models;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.Brands.Queries.Models;

public record GetBrandsQuery() : IRequest<IResult<IEnumerable<SelectOption>>>;