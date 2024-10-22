using smERP.Domain.Entities.ExternalEntities;
using smERP.Application.Contracts.Persistence;
using smERP.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using smERP.Application.Features.Attributes.Queries.Responses;
using smERP.Application.Features.Branches.Queries.Models;
using smERP.SharedKernel.Responses;
using smERP.Application.Features.Suppliers.Queries.Responses;
using smERP.Application.Features.Suppliers.Commands.Models;

namespace smERP.Persistence.Repositories;

public class SupplierRepository(ProductDbContext context) : Repository<Supplier>(context), ISupplierRepository
{
    public new async Task<PagedResult<GetPaginatedSuppliersQueryResponse>> GetPagedAsync(PaginationParameters parameters)
    {
        var query = _context.Set<Supplier>().AsQueryable();

        var filteredQuery = ApplyFilters(query, parameters);
        var totalCount = await filteredQuery.CountAsync();

        var sortedQuery = ApplySorting(filteredQuery, parameters);

        var paginatedQuery = sortedQuery
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize);

        var projectedQuery = paginatedQuery.Select(b => new GetPaginatedSuppliersQueryResponse(
            b.Id,
            b.Name.English,
            b.Addresses.Select(address => new Address(address.Street, address.City, address.State, address.Country, address.PostalCode, address.Comment)),
            b.SuppliedProducts.Select(product => new SuppliedProduct(product.ProductId, product.Product.Name.English, product.FirstTimeSupplied, product.LastTimeSupplied))
        ));

        var pagedData = await projectedQuery.ToListAsync();

        return new PagedResult<GetPaginatedSuppliersQueryResponse>
        {
            TotalCount = totalCount,
            PageSize = parameters.PageSize,
            PageNumber = parameters.PageNumber,
            Data = pagedData
        };
    }

    private static IQueryable<Supplier> ApplyFilters(IQueryable<Supplier> query, PaginationParameters parameters)
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

    private static IQueryable<Supplier> ApplySorting(IQueryable<Supplier> query, PaginationParameters parameters)
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

    public async Task<IEnumerable<SelectOption>> GetSuppliers()
    {
        return await _context.Set<Supplier>().Select(supplier => new SelectOption(supplier.Id, supplier.Name.English)).ToArrayAsync();
    }
}
