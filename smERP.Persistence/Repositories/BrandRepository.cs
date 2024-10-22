using smERP.Domain.Entities.Product;
using smERP.Application.Contracts.Persistence;
using smERP.Persistence.Data;
using smERP.SharedKernel.Responses;
using smERP.Application.Features.Brands.Queries.Responses;
using Microsoft.EntityFrameworkCore;
using smERP.Application.Features.Branches.Queries.Models;

namespace smERP.Persistence.Repositories;

public class BrandRepository(ProductDbContext context) : Repository<Brand>(context), IBrandRepository
{
    public new async Task<PagedResult<GetPaginatedBrandsQueryResponse>> GetPagedAsync(PaginationParameters parameters)
    {
        var query = _context.Set<Brand>().AsQueryable();

        var filteredQuery = ApplyFilters(query, parameters);
        var totalCount = await filteredQuery.CountAsync();

        var sortedQuery = ApplySorting(filteredQuery, parameters);

        var paginatedQuery = sortedQuery
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize);

        var projectedQuery = paginatedQuery.Select(b => new GetPaginatedBrandsQueryResponse(
            b.Id,
            b.Name.English,
            b.Products.Count()
        ));

        var pagedData = await projectedQuery.ToListAsync();

        return new PagedResult<GetPaginatedBrandsQueryResponse>
        {
            TotalCount = totalCount,
            PageSize = parameters.PageSize,
            PageNumber = parameters.PageNumber,
            Data = pagedData
        };
    }

    private static IQueryable<Brand> ApplyFilters(IQueryable<Brand> query, PaginationParameters parameters)
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

    private static IQueryable<Brand> ApplySorting(IQueryable<Brand> query, PaginationParameters parameters)
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

    public async Task<IEnumerable<SelectOption>> GetBrands()
    {
        return await _context.Set<Brand>().Select(brand => new SelectOption(brand.Id, brand.Name.English)).ToListAsync();
    }
}
