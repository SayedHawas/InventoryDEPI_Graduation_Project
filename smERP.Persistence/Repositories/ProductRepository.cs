using Microsoft.EntityFrameworkCore;
using smERP.Domain.Entities;
using smERP.Domain.Entities.Product;
using smERP.Application.Contracts.Persistence;
using smERP.Persistence.Data;
using System.Linq;
using smERP.SharedKernel.Responses;
using smERP.Application.Features.Products.Queries.Models;
using smERP.Application.Features.Products.Queries.Responses;
using ProductInstance = smERP.Domain.Entities.Product.ProductInstance;
using smERP.Application.Results.Persistence;
using System.Text;
using GetProductInstance = smERP.Application.Results.Persistence.GetProductInstance;

namespace smERP.Persistence.Repositories;

public class ProductRepository(ProductDbContext context) : Repository<Product>(context), IProductRepository
{
    public async Task<Product?> GetByIdWithProductInstances(int productId)
    {
        return await context.Products.Include(x => x.ProductInstances).FirstOrDefaultAsync(x => x.Id == productId);
    }

    public async Task<IEnumerable<(int ProductInstanceId,bool IsTracked, bool IsWarranted, int? ShelfLifeInDays)>> GetProductInstancesWithProduct(IEnumerable<int> productinstanceIds)
    {
        return await _context.Set<ProductInstance>()
            .Include(x => x.Product)
            .Where(x => productinstanceIds.Contains(x.Id))
            .Select(x => new ValueTuple<int,bool, bool, int?>(
                x.Id,
                x.Product.AreItemsTracked,
                x.Product.IsWarranted,
                x.Product.ShelfLifeInDays
            ))
            .ToListAsync();
    }

    public new async Task<PagedResult<GetPaginatedProductsQueryResponse>> GetPagedAsync(GetPaginatedProductsQuery parameters)
    {
        var query = _context.Set<Product>().AsQueryable();

        var filteredQuery = ApplyFilters(query, parameters);
        var totalCount = await filteredQuery.CountAsync();

        var sortedQuery = ApplySorting(filteredQuery, parameters);

        var paginatedQuery = sortedQuery
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize);

        var projectedQuery = paginatedQuery.Select(b => new GetPaginatedProductsQueryResponse(
            b.Id,
            b.ModelNumber,
            b.Name.English,
            b.AreItemsTracked,
            b.ShelfLifeInDays,
            b.WarrantyInDays,
            b.Brand.Name.English,
            b.Category.Name.English,
            b.ProductInstances.Select(instance => new Application.Features.Products.Queries.Responses.ProductInstance(instance.Id,
                                                                                                                      instance.Sku,
                                                                                                                      instance.QuantityInStock,
                                                                                                                      instance.BuyingPrice,
                                                                                                                      instance.SellingPrice,
                                                                                                                      instance.Images.First().Path)).ToList()
        ));

        var pagedData = await projectedQuery.ToListAsync();

        return new PagedResult<GetPaginatedProductsQueryResponse>
        {
            TotalCount = totalCount,
            PageSize = parameters.PageSize,
            PageNumber = parameters.PageNumber,
            Data = pagedData
        };
    }

    public IQueryable<Product> ApplyFilters(IQueryable<Product> query, GetPaginatedProductsQuery parameters)
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
        if (parameters.CategoryId.HasValue)
        {
            var categoryIds = GetCategoryAndDescendantIds(parameters.CategoryId.Value);
            query = query.Where(p => categoryIds.Contains(p.CategoryId));
        }
        if (parameters.BrandId.HasValue)
        {
            query = query.Where(b => b.BrandId == parameters.BrandId.Value);
        }
        if (parameters.Attributes != null && parameters.Attributes.Count > 0)
        {
            query = query.Where(b => b.ProductInstances
                .Any(pi => pi.ProductInstanceAttributeValues
                    .Any(piav => parameters.Attributes.Contains(piav.AttributeValueId))));
        }
        return query;
    }

    private HashSet<int> GetCategoryAndDescendantIds(int categoryId)
    {
        var result = new HashSet<int> { categoryId };
        var queue = new Queue<int>();
        queue.Enqueue(categoryId);

        while (queue.Count > 0)
        {
            var currentId = queue.Dequeue();
            var children = _context.Set<Category>()
                .Where(c => c.ParentCategoryId == currentId)
                .Select(c => c.Id)
                .ToList();

            foreach (var childId in children)
            {
                if (result.Add(childId))
                {
                    queue.Enqueue(childId);
                }
            }
        }

        return result;
    }

    private static IQueryable<Product> ApplySorting(IQueryable<Product> query, GetPaginatedProductsQuery parameters)
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

    public async Task<ProductInstance?> GetProductInstance(int productId, int productInstanceId)
    {
        return await _context.Set<ProductInstance>().FirstOrDefaultAsync(x => x.ProductId == productId && x.Id == productInstanceId);
    }

    public async Task<IReadOnlyList<GetProductsQueryResponse>> GetProducts()
    {
        return await _context.Set<ProductInstance>()
            .AsNoTracking()
            .Select(instance => new
            {
                instance.ProductId,
                instance.Id,
                instance.Product.ShelfLifeInDays,
                IsWarranted = instance.Product.WarrantyInDays != null,
                ProductName = instance.Product.Name.English,
                Attributes = instance.ProductInstanceAttributeValues.Select(x => new
                {
                    AttributeName = x.AttributeValue.Attribute.Name.English,
                    AttributeValue = x.AttributeValue.Value.English
                }).ToList()
            })
            .ToListAsync()
            .ContinueWith(task =>
            {
                return task.Result.Select(item =>
                {
                    var sb = new StringBuilder(item.ProductName);
                    foreach (var attr in item.Attributes)
                    {
                        sb.Append(" (").Append(attr.AttributeName).Append(": ").Append(attr.AttributeValue).Append(')');
                    }
                    return new GetProductsQueryResponse(item.ProductId, item.Id, sb.ToString(), item.ShelfLifeInDays, item.IsWarranted);
                }).ToList();
            });
    }

    public async Task<GetProductInstance?> GetProductInstance(int productInstanceId)
    {
        return await _context.Set<ProductInstance>().AsNoTracking().Where(x => x.Id == productInstanceId)
            .Select(instance => new GetProductInstance(instance.ProductId, instance.Id, instance.Product.WarrantyInDays.HasValue || instance.Product.ShelfLifeInDays.HasValue, instance.Product.ShelfLifeInDays))
            .FirstOrDefaultAsync();
    }

    public async Task<bool> DoesProductInstanceExist(int productInstanceId)
    {
        return await _context.Set<ProductInstance>().AnyAsync(x => x.Id == productInstanceId);
    }

    public async Task<List<(int ProductInstanceId, string ProductInstanceName)>> GetProductNames(List<int> productInstanceIds)
    {
        return await _context.Set<ProductInstance>()
            .AsNoTracking()
            .Select(instance => new
            {
                ProductInstanceId = instance.Id,
                ProductName = instance.Product.Name.English,
                Attributes = instance.ProductInstanceAttributeValues.Select(x => new
                {
                    AttributeName = x.AttributeValue.Attribute.Name.English,
                    AttributeValue = x.AttributeValue.Value.English
                }).ToList()
            })
            .ToListAsync()
            .ContinueWith(task =>
            {
                return task.Result.Select(item =>
                {
                    var sb = new StringBuilder(item.ProductName);
                    foreach (var attr in item.Attributes)
                    {
                        sb.Append(" (").Append(attr.AttributeName).Append(": ").Append(attr.AttributeValue).Append(')');
                    }
                    return (item.ProductInstanceId, sb.ToString());
                }).ToList();
            });
    }
}
