using smERP.Application.Features.Branches.Queries.Models;
using smERP.Application.Features.Branches.Queries.Responses;
using smERP.Domain.Entities.Organization;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Contracts.Persistence;

public interface IBranchRepository : IRepository<Branch>
{
    Task<IEnumerable<SelectOption>> GetAllBranches();
    Task<Branch> GetByIdWithStorageLocations(int Id);
    new Task<PagedResult<GetPaginatedBranchesQueryResponse>> GetPagedAsync(PaginationParameters parameters);
    Task<GetStorageLocationQueryResponse?> GetStorageLocation(int StorageLocationId);
    Task<IEnumerable<GetBranchesWithStorageLocationsQueryResponse>> GetBranchesWithStorageLocations();
    Task<PagedResult<GetPaginatedStorageLocationsQueryResponse>> GetPaginatedStorageLocations(GetPaginatedStorageLocationsQuery parameters);
        }