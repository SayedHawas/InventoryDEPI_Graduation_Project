namespace smERP.Application.Features.Branches.Queries.Responses;

public record GetStorageLocationQueryResponse(int BranchId, int StorageLocationId, string Name);