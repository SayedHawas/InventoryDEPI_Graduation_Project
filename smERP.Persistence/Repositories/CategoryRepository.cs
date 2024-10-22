using smERP.Domain.Entities.Product;
using smERP.Application.Contracts.Persistence;
using smERP.Persistence.Data;
using smERP.SharedKernel.Responses;
using smERP.Application.Features.Categories.Queries.Responses;
using Microsoft.EntityFrameworkCore;
using smERP.Application.Features.Brands.Queries.Responses;
using smERP.Application.Features.Branches.Queries.Models;

namespace smERP.Persistence.Repositories;

public class CategoryRepository(ProductDbContext context) : Repository<Category>(context), ICategoryRepository
{
    public new async Task<PagedResult<GetPaginatedCategoriesQueryResponse>> GetPagedAsync(PaginationParameters parameters)
    {
        var query = _context.Set<Category>().AsQueryable();

        var filteredQuery = ApplyFilters(query, parameters);
        var totalCount = await filteredQuery.CountAsync();

        var sortedQuery = ApplySorting(filteredQuery, parameters);

        var paginatedQuery = sortedQuery
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize);

        var projectedQuery = paginatedQuery.Select(b => new GetPaginatedCategoriesQueryResponse(
            b.Id,
            b.Name.English,
            b.Name.Arabic,
            b.ProductCount,
            b.ParentCategory != null ? new SelectOption(b.ParentCategory.Id, b.ParentCategory.Name.English) : null,
            b.InverseParentCategory.Select(category => new SelectOption(category.Id, category.Name.English)).ToList()
        ));

        var pagedData = await projectedQuery.ToListAsync();

        return new PagedResult<GetPaginatedCategoriesQueryResponse>
        {
            TotalCount = totalCount,
            PageSize = parameters.PageSize,
            PageNumber = parameters.PageNumber,
            Data = pagedData
        };
    }

    private static IQueryable<Category> ApplyFilters(IQueryable<Category> query, PaginationParameters parameters)
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

    private static IQueryable<Category> ApplySorting(IQueryable<Category> query, PaginationParameters parameters)
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

    public async Task<IEnumerable<SelectOption>> GetParentCategories()
    {
        return await _context.Set<Category>().Where(x => x.ProductCount == 0).Select(category => new SelectOption(category.Id, category.Name.English)).ToListAsync();
    }

    public async Task<IEnumerable<SelectOption>> GetProductCategories()
    {
        return await _context.Set<Category>().Where(x => x.IsLeaf).Select(category => new SelectOption(category.Id, category.Name.English)).ToListAsync();
    }
}
