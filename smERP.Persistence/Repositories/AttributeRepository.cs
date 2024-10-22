using Microsoft.EntityFrameworkCore;
using smERP.Domain.Entities.Product;
using smERP.Application.Contracts.Persistence;
using smERP.Persistence.Data;
using Attribute = smERP.Domain.Entities.Product.Attribute;
using smERP.Application.Features.Attributes.Queries.Responses;
using smERP.Application.Features.Branches.Queries.Models;
using smERP.SharedKernel.Responses;
using smERP.Application.Features.Brands.Queries.Responses;

namespace smERP.Persistence.Repositories;

public class AttributeRepository(ProductDbContext context) : Repository<Attribute>(context), IAttributeRepository
{
    public async Task<bool> DoesListExist(List<(int AttributeId, int AttributeValueId)> attributeValues)
    {
        if (attributeValues.Count == 0) return true;

        var existingCount = await _context.Set<AttributeValue>()
            .CountAsync(av => attributeValues.Select(x => x.AttributeId).Contains(av.AttributeId) && attributeValues.Select(x => x.AttributeValueId).Contains(av.Id));

        return existingCount == attributeValues.Count;
    }

    public async Task<IEnumerable<GetAttributesQueryResponse>> GetAttributesSelectionList()
    {
        var attributesSelectionList = await _context.Set<Attribute>().Select(attribute =>
            new GetAttributesQueryResponse(
                attribute.Id,
                attribute.Name.English,
                attribute.AttributeValues.Select(attributeValue => new SelectOption(attributeValue.Id, attributeValue.Value.English))))
            .ToListAsync();

        return attributesSelectionList;
    }

    public override async Task<Attribute> GetByID(int id)
    {
        return await context.Attributes.Include(x => x.AttributeValues).FirstOrDefaultAsync(a => a.Id == id);
    }

    public new async Task<PagedResult<GetAttributesQueryResponse>> GetPagedAsync(PaginationParameters parameters)
    {
        var query = _context.Set<Attribute>().AsQueryable();

        var filteredQuery = ApplyFilters(query, parameters);
        var totalCount = await filteredQuery.CountAsync();

        var sortedQuery = ApplySorting(filteredQuery, parameters);

        var paginatedQuery = sortedQuery
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize);

        var projectedQuery = paginatedQuery.Select(b => new GetAttributesQueryResponse(
            b.Id,
            b.Name.English,
            b.AttributeValues.Select(value => new SelectOption(value.Id, value.Value.English))
        ));

        var pagedData = await projectedQuery.ToListAsync();

        return new PagedResult<GetAttributesQueryResponse>
        {
            TotalCount = totalCount,
            PageSize = parameters.PageSize,
            PageNumber = parameters.PageNumber,
            Data = pagedData
        };
    }

    private static IQueryable<Attribute> ApplyFilters(IQueryable<Attribute> query, PaginationParameters parameters)
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

    private static IQueryable<Attribute> ApplySorting(IQueryable<Attribute> query, PaginationParameters parameters)
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

}
