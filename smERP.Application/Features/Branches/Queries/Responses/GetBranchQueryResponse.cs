using smERP.Application.Features.Branches.Queries.Models;

namespace smERP.Application.Features.Branches.Queries.Responses;

public record GetBranchQueryResponse(
    int BranchId,
    string EnglishName,
    string ArabicName,
    List<SelectOption> StorageLocations
    );