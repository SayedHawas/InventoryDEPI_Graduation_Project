using Microsoft.EntityFrameworkCore;
using smERP.Domain.Entities.Product;
using smERP.SharedKernel.Responses;
using System.Reflection;
using System.Linq.Expressions;
using smERP.Domain.ValueObjects;

public static class IQueryableExtensions
{
    public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
        this IQueryable<T> query,
        PaginationParameters parameters)
    {
        var totalCount = await query.CountAsync();

        if (!string.IsNullOrEmpty(parameters.SearchTerm))
        {
            query = ApplyBilingualNameSearch(query, parameters.SearchTerm);
        }

        if (parameters.StartDate.HasValue)
        {
            query = query.Where(e => EF.Property<DateTime>(e, "CreatedAt") >= parameters.StartDate.Value);
        }

        if (parameters.EndDate.HasValue)
        {
            query = query.Where(e => EF.Property<DateTime>(e, "CreatedAt") <= parameters.EndDate.Value);
        }

        if (!string.IsNullOrEmpty(parameters.SortBy))
        {
            query = ApplySorting(query, parameters.SortBy, parameters.SortDescending);
        }

        var pagedData = await query
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();

        return new PagedResult<T>
        {
            TotalCount = totalCount,
            PageNumber = parameters.PageNumber,
            PageSize = parameters.PageSize,
            Data = pagedData
        };
    }

    private static IQueryable<T> ApplyBilingualNameSearch<T>(IQueryable<T> query, string searchTerm)
    {
        var type = typeof(T);
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var bilingualNameProperties = properties.Where(p => p.PropertyType == typeof(BilingualName)).ToList();

        if (bilingualNameProperties.Any())
        {
            var parameter = Expression.Parameter(type, "x");
            var searchTermExpression = Expression.Constant(searchTerm);

            var orExpressions = bilingualNameProperties.SelectMany(prop =>
            {
                var nameProperty = Expression.Property(parameter, prop);
                var englishProperty = Expression.Property(nameProperty, "English");
                var arabicProperty = Expression.Property(nameProperty, "Arabic");

                var englishLike = Expression.Call(typeof(DbFunctionsExtensions), "Like",
                    Type.EmptyTypes, Expression.Property(null, typeof(EF), "Functions"), englishProperty, Expression.Constant($"%{searchTerm}%"));

                var arabicLike = Expression.Call(typeof(DbFunctionsExtensions), "Like",
                    Type.EmptyTypes, Expression.Property(null, typeof(EF), "Functions"), arabicProperty, Expression.Constant($"%{searchTerm}%"));

                return new Expression[] { englishLike, arabicLike };
            }).ToList();

            var combinedOrExpression = orExpressions.Aggregate(Expression.OrElse);
            var lambda = Expression.Lambda<Func<T, bool>>(combinedOrExpression, parameter);

            return query.Where(lambda);
        }

        return query.Where(e => EF.Functions.Like(e.ToString(), $"%{searchTerm}%"));
    }

    private static IQueryable<T> ApplySorting<T>(IQueryable<T> query, string sortBy, bool sortDescending)
    {
        var type = typeof(T);
        var property = type.GetProperty(sortBy, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

        if (property != null)
        {
            var parameter = Expression.Parameter(type, "x");
            var propertyAccess = Expression.Property(parameter, property);
            var orderByExp = Expression.Lambda(propertyAccess, parameter);

            var methodName = sortDescending ? "OrderByDescending" : "OrderBy";
            var resultExp = Expression.Call(typeof(Queryable), methodName,
                new Type[] { type, property.PropertyType },
                query.Expression, Expression.Quote(orderByExp));

            return query.Provider.CreateQuery<T>(resultExp);
        }

        return query;
    }
}