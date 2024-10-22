using smERP.Application.Features.Branches.Queries.Models;

namespace smERP.Application.Features.Attributes.Queries.Responses;

public record GetAttributesQueryResponse(int Value, string Label, IEnumerable<SelectOption> Values);