using Microsoft.EntityFrameworkCore;
using smERP.Domain.Entities.Organization;
using smERP.Application.Contracts.Persistence;
using smERP.Persistence.Data;
using smERP.Application.Features.Branches.Queries.Models;
using smERP.Application.Features.Brands.Queries.Responses;
using smERP.Domain.Entities.Product;
using smERP.SharedKernel.Responses;
using smERP.Application.Features.Branches.Queries.Responses;
using System.Linq;
using System.Threading.Tasks.Dataflow;

namespace smERP.Persistence.Repositories;

public class BranchRepository(ProductDbContext context) : Repository<Branch>(context), IBranchRepository
{
    public async Task<IEnumerable<SelectOption>> GetAllBranches()
    {
        return await context.Branches.Select(branch => new SelectOption(branch.Id, branch.Name.English)).ToListAsync();
    }

    public async Task<Branch> GetByIdWithStorageLocations(int Id)
    {
        return await context.Branches.Include(x => x.StorageLocations).FirstOrDefaultAsync(x => x.Id == Id);
    }

    public new async Task<PagedResult<GetPaginatedBranchesQueryResponse>> GetPagedAsync(PaginationParameters parameters)
    {
        var query = _context.Set<Branch>().AsQueryable();

        var filteredQuery = ApplyFilters(query, parameters);
        var totalCount = await filteredQuery.CountAsync();

        var sortedQuery = ApplySorting(filteredQuery, parameters);

        var paginatedQuery = sortedQuery
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize);

        var projectedQuery = paginatedQuery.Select(b => new GetPaginatedBranchesQueryResponse(
            b.Id,
            b.Name.English,
            b.StorageLocations.Select(storage => new SelectOption(storage.Id, storage.Name)).ToList()
        ));

        var pagedData = await projectedQuery.ToListAsync();

        return new PagedResult<GetPaginatedBranchesQueryResponse>
        {
            TotalCount = totalCount,
            PageSize = parameters.PageSize,
            PageNumber = parameters.PageNumber,
            Data = pagedData
        };
    }

    private static IQueryable<Branch> ApplyFilters(IQueryable<Branch> query, PaginationParameters parameters)
    {
        if (!string.IsNullOrEmpty(parameters.SearchTerm))
        {
            query = query.Where(b =>
                EF.Functions.Like(b.Name.English, $"%{parameters.SearchTerm}%") ||
                EF.Functions.Like(b.Name.Arabic, $"%{parameters.SearchTerm}%"));
        }

        if (parameters.StartDate.HasValue)
        {
            query = query.Where(b => b.CreatedAt >= parameters.StartDate.Value);
        }

        if (parameters.EndDate.HasValue)
        {
            query = query.Where(b => b.CreatedAt <= parameters.EndDate.Value);
        }

        return query;
    }

    private static IQueryable<Branch> ApplySorting(IQueryable<Branch> query, PaginationParameters parameters)
    {
        if (!string.IsNullOrEmpty(parameters.SortBy))
        {
            switch (parameters.SortBy.ToLower())
            {
                case "name":
                    query = parameters.SortDescending
                        ? query.OrderByDescending(b => b.Name.English)
                        : query.OrderBy(b => b.Name.English);
                    break;
                case "createdat":
                    query = parameters.SortDescending
                        ? query.OrderByDescending(b => b.CreatedAt)
                        : query.OrderBy(b => b.CreatedAt);
                    break;
            }
        }
        else
        {
            query = query.OrderBy(b => b.Id);
        }

        return query;
    }

    public async Task<GetStorageLocationQueryResponse?> GetStorageLocation(int StorageLocationId)
    {
        return await _context.Set<StorageLocation>()
            .Where(x => x.Id == StorageLocationId)
            .Select(
            location => new GetStorageLocationQueryResponse(location.BranchId, location.Id, location.Name))
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<GetBranchesWithStorageLocationsQueryResponse>> GetBranchesWithStorageLocations()
    {
        return await _context.Set<Branch>()
            .Select(branch => new GetBranchesWithStorageLocationsQueryResponse(branch.Id, branch.Name.English,
            branch.StorageLocations.Select(storage => new StorageLocationOption(storage.Id, storage.Name)))).ToArrayAsync();
    }

    public async Task<PagedResult<GetPaginatedStorageLocationsQueryResponse>> GetPaginatedStorageLocations(GetPaginatedStorageLocationsQuery parameters)
    {
        var query = _context.Set<StorageLocation>().Where(sl => sl.BranchId == parameters.BranchId).AsQueryable();

        var filteredQuery = ApplyStorageLocationFilters(query, parameters);
        var totalCount = await filteredQuery.CountAsync();
        var sortedQuery = ApplyStorageLocationSorting(filteredQuery, parameters);

        var paginatedQuery = sortedQuery
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize);

        var pagedData = await paginatedQuery
            .Select(sl => new GetPaginatedStorageLocationsQueryResponse(
                sl.Id,
                sl.Name,
                sl.StoredProductInstances.Select(spi => new ProductStored(
                    spi.ProductInstanceId,
                    spi.ProductInstance.Sku ?? "",
                    spi.ProductInstance.Product.Name.English,
                    spi.Quantity,
                    sl.ProcurementTransactions
                        .SelectMany(pt => pt.Items)
                        .Where(item => item.ProductInstanceId == spi.ProductInstanceId)
                        .GroupBy(item => item.ProductInstanceId)
                        .Select(g => g.Average(item => (item.UnitPrice * item.Quantity) / item.Quantity))
                        .FirstOrDefault(),
                    spi.ProductInstance.SellingPrice
                ))
            ))
            .ToListAsync();

        return new PagedResult<GetPaginatedStorageLocationsQueryResponse>
        {
            TotalCount = totalCount,
            PageSize = parameters.PageSize,
            PageNumber = parameters.PageNumber,
            Data = pagedData
        };
    }

    private static IQueryable<StorageLocation> ApplyStorageLocationFilters(IQueryable<StorageLocation> query, GetPaginatedStorageLocationsQuery parameters)
    {
        if (!string.IsNullOrEmpty(parameters.SearchTerm))
        {
            query = query.Where(sl =>
                EF.Functions.Like(sl.Name, $"%{parameters.SearchTerm}%") ||
                sl.StoredProductInstances.Any(spi =>
                    EF.Functions.Like(spi.ProductInstance.Product.Name.English, $"%{parameters.SearchTerm}%")));
        }
        return query;
    }

    private static IQueryable<StorageLocation> ApplyStorageLocationSorting(IQueryable<StorageLocation> query, GetPaginatedStorageLocationsQuery parameters)
    {
        if (!string.IsNullOrEmpty(parameters.SortBy))
        {
            switch (parameters.SortBy.ToLower())
            {
                case "name":
                    query = parameters.SortDescending
                        ? query.OrderByDescending(sl => sl.Name)
                        : query.OrderBy(sl => sl.Name);
                    break;
                case "createdat":
                    query = parameters.SortDescending
                        ? query.OrderByDescending(sl => sl.CreatedAt)
                        : query.OrderBy(sl => sl.CreatedAt);
                    break;
                default:
                    query = query.OrderBy(sl => sl.Id);
                    break;
            }
        }
        else
        {
            query = query.OrderBy(sl => sl.Id);
        }
        return query;
    }


}