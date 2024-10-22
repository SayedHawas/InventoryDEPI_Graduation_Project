using smERP.Application.Features.Branches.Queries.Models;
using smERP.Application.Features.Categories.Queries.Responses;
using smERP.Domain.Entities.Product;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Contracts.Persistence;

public interface ICategoryRepository : IRepository<Category>
{
    Task<IEnumerable<SelectOption>> GetParentCategories();
    Task<IEnumerable<SelectOption>> GetProductCategories();
    new Task<PagedResult<GetPaginatedCategoriesQueryResponse>> GetPagedAsync(PaginationParameters parameters);
}

