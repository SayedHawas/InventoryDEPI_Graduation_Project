using smERP.SharedKernel.Responses;
using System.Linq.Expressions;

namespace smERP.Application.Contracts.Persistence;

public interface IRepository<T> where T : class
{
    Task<T> GetByID(int ID);
    Task<IEnumerable<T>> GetListByIds(IEnumerable<int> Ids);
    Task<IEnumerable<T>> GetAll();
    Task<T> Add(T entity, CancellationToken cancellationToken = default);
    Task<bool> DoesExist(int ID);
    Task<bool> DoesExist(Expression<Func<T, bool>> predicate);
    Task<bool> IsTableEmpty();
    Task<int> CountExisting(IEnumerable<int> IDs);
    void Update(T entity);
    void Remove(T entity);

    Task<PagedResult<T>> GetPagedAsync(PaginationParameters parameters);

    Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> criteria, int? take, int? skip,
    Expression<Func<T, object>> orderBy = null, string orderByDirection = "asc");
    //Task Hide(int ID);
}
