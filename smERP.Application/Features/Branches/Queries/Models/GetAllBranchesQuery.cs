using MediatR;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.Branches.Queries.Models;

public record GetAllBranchesQuery() : IRequest<IResult<IEnumerable<SelectOption>>>;

public record SelectOption(int Value, string Label);