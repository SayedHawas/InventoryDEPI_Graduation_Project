using smERP.Application.Features.Products.Queries.Models;
using smERP.Application.Features.Products.Queries.Responses;
using smERP.Application.Results.Persistence;
using smERP.Domain.Entities.Product;
using smERP.SharedKernel.Responses;
using GetProductInstance = smERP.Application.Results.Persistence.GetProductInstance;
using ProductInstance = smERP.Domain.Entities.Product.ProductInstance;

namespace smERP.Application.Contracts.Persistence;

public interface IProductRepository : IRepository<Product>
{
    Task<Product?> GetByIdWithProductInstances(int productId);
    Task<ProductInstance?> GetProductInstance(int productId, int productInstanceId);
    Task<GetProductInstance?> GetProductInstance(int productInstanceId);
    Task<bool> DoesProductInstanceExist(int productInstanceId);
    Task<IReadOnlyList<GetProductsQueryResponse>> GetProducts();
    Task<IEnumerable<(int ProductInstanceId, bool IsTracked, bool IsWarranted, int? ShelfLifeInDays)>> GetProductInstancesWithProduct(IEnumerable<int> productinstanceIds);
    Task<PagedResult<GetPaginatedProductsQueryResponse>> GetPagedAsync(GetPaginatedProductsQuery parameters);
    Task<List<(int ProductInstanceId,string ProductInstanceName)>> GetProductNames(List<int> productInstanceIds);
}
