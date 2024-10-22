namespace smERP.Application.Features.Branches.Queries.Responses;

public record GetBranchesWithStorageLocationsQueryResponse(
    int BranchId,
    string Name,
    IEnumerable<StorageLocationOption> StorageLocations
    );

public record StorageLocationOption(int StorageLocationId, string Name);