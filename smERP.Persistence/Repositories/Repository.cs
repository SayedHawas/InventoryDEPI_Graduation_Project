using Microsoft.EntityFrameworkCore;
using smERP.Domain.Entities;
using smERP.Application.Contracts.Persistence;
using smERP.Persistence.Data;
using smERP.SharedKernel.Bases;
using System.Linq.Expressions;
using smERP.SharedKernel.Responses;

namespace smERP.Persistence.Repositories;

public class Repository<TEntity>(DbContext context) : IRepository<TEntity> where TEntity : Entity
{
    protected readonly DbContext _context = context;

    public virtual async Task<TEntity> GetByID(int ID)
    {
        return await _context.Set<TEntity>().AsTracking().FirstOrDefaultAsync(x => x.Id == ID);
    }

    public virtual async Task<IEnumerable<TEntity>> GetListByIds(IEnumerable<int> Ids)
    {
        return await _context.Set<TEntity>().Where(x => Ids.Contains(x.Id)).ToListAsync();
    }

    public async Task<IEnumerable<TEntity>> GetAll()
    {
        return await _context.Set<TEntity>().ToListAsync();
    }

    public virtual async Task<PagedResult<TEntity>> GetPagedAsync(PaginationParameters parameters)
    {
        return await _context.Set<TEntity>().AsQueryable().ToPagedResultAsync(parameters);
    }

    public IQueryable<TEntity> FilterData(Func<IQueryable<TEntity>, IQueryable<TEntity>> filterFunc, BaseFiltration parameters)
    {
        var query = _context.Set<TEntity>().AsQueryable();

        query = filterFunc(query);

        if (!string.IsNullOrEmpty(parameters.SearchText))
        {
            query = query.Where(b => EF.Property<string>(b, "Name").Contains(parameters.SearchText));
        }

        if (!string.IsNullOrEmpty(parameters.FromDate) && DateTime.TryParse(parameters.FromDate, out var fromDate))
        {
            query = query.Where(f => EF.Property<DateTime>(f, "CreatedDate").Date >= fromDate.Date);
        }

        if (!string.IsNullOrEmpty(parameters.ToDate) && DateTime.TryParse(parameters.ToDate, out var toDate))
        {
            query = query.Where(f => EF.Property<DateTime>(f, "CreatedDate").Date <= toDate.Date);
        }

        // Apply paging directly to the query
        //var pagedData = query
        //    .Skip(parameters.Start)
        //    .Take(parameters.Length)
        //    .ToList();

        return query;
    }

    public async Task<TEntity> Add(TEntity entity, CancellationToken cancellationToken = default)
    {
        await _context.Set<TEntity>().AddAsync(entity, cancellationToken);
        return entity;
    }

    public void Update(TEntity entity)
    {
        _context.Set<TEntity>().Update(entity);
    }

    //public async Task Hide(int ID)
    //{
    //    var entityToBeHidden = await _context.Set<TEntity>().AsTracking().FirstOrDefaultAsync(x => x.Id == ID);
    //    if (entityToBeHidden is not null)
    //        entityToBeHidden.IsHidden = true;
    //}

    public async Task<bool> DoesExist(int ID)
    {
        return await _context.Set<TEntity>().AnyAsync(x => x.Id == ID);
    }

    public async Task<bool> DoesExist(Expression<Func<TEntity, bool>> predicate)
    {
        return await _context.Set<TEntity>().AsNoTracking().AnyAsync(predicate);
    }

    public async Task<bool> IsTableEmpty()
    {
        return !await _context.Set<TEntity>().AsNoTracking().AnyAsync();
    }

    public async Task<int> CountExisting(IEnumerable<int> IDs)
    {
        return await _context.Set<TEntity>().CountAsync(x => IDs.Contains(x.Id));
    }

    public void Remove(TEntity entity)
    {
        _context.Set<TEntity>().Remove(entity);
    }

    public async Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> criteria, int? take, int? skip,
    Expression<Func<TEntity, object>> orderBy = null, string orderByDirection = "asc")
    {
        IQueryable<TEntity> query = _context.Set<TEntity>().Where(criteria);

        if (take.HasValue)
            query = query.Take(take.Value);

        if (skip.HasValue)
            query = query.Skip(skip.Value);

        if (orderBy != null)
        {
            if (orderByDirection == "asc")
                query = query.OrderBy(orderBy);
            else
                query = query.OrderByDescending(orderBy);
        }

        return await query.ToListAsync();
    }
}

