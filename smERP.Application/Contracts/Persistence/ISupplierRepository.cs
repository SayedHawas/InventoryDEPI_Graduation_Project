using smERP.Application.Features.Branches.Queries.Models;
using smERP.Application.Features.Suppliers.Queries.Responses;
using smERP.Domain.Entities.ExternalEntities;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Contracts.Persistence;

public interface ISupplierRepository : IRepository<Supplier>
{
    new Task<PagedResult<GetPaginatedSuppliersQueryResponse>> GetPagedAsync(PaginationParameters parameters);
    Task<IEnumerable<SelectOption>> GetSuppliers();
}

