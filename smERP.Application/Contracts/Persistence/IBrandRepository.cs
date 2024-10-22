using smERP.Application.Features.Branches.Queries.Models;
using smERP.Application.Features.Brands.Queries.Responses;
using smERP.Domain.Entities.Product;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Contracts.Persistence;

public interface IBrandRepository : IRepository<Brand>
{
    new Task<PagedResult<GetPaginatedBrandsQueryResponse>> GetPagedAsync(PaginationParameters parameters);
    Task<IEnumerable<SelectOption>> GetBrands();
}
