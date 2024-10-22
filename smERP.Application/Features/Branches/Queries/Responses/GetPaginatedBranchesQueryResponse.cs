using smERP.Application.Features.Branches.Queries.Models;

namespace smERP.Application.Features.Branches.Queries.Responses;

public record GetPaginatedBranchesQueryResponse(int BranchId, string Name, List<SelectOption> StorageLocations);