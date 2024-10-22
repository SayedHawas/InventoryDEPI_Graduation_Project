using MediatR;
using smERP.Application.Contracts.Persistence;
using smERP.Application.Features.Products.Queries.Models;
using smERP.Application.Features.Products.Queries.Responses;
using smERP.Domain.Entities.Product;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.Products.Queries.Handlers;

public class ProductQueryHandler(IProductRepository productRepository) :
    IRequestHandler<GetPaginatedProductsQuery, IResult<PagedResult<GetPaginatedProductsQueryResponse>>>,
    IRequestHandler<GetProductQuery, IResult<GetProductQueryResponse>>,
    IRequestHandler<GetProductInstanceQuery, IResult<GetProductInstanceQueryResponse>>,
    IRequestHandler<GetProductsQuery, IResult<IEnumerable<GetProductsQueryResponse>>>
{
    private readonly IProductRepository _productRepository = productRepository;

    public async Task<IResult<PagedResult<GetPaginatedProductsQueryResponse>>> Handle(GetPaginatedProductsQuery request, CancellationToken cancellationToken)
    {
        var paginatedProducts = await _productRepository.GetPagedAsync(request);
        return new Result<PagedResult<GetPaginatedProductsQueryResponse>>(paginatedProducts);
    }

    public async Task<IResult<GetProductQueryResponse>> Handle(GetProductQuery request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdWithProductInstances(request.ProductId);
        if (product == null)
            return new Result<GetProductQueryResponse>().WithNotFound();

        var productResponse = new GetProductQueryResponse(product.Id, product.Name.English, product.Name.English, product.ModelNumber, product.Description ?? "", product.ShelfLifeInDays, product.WarrantyInDays, product.BrandId, product.CategoryId,
            product.ProductInstances.Select(instance => new GetProductInstance(instance.Id, instance.Sku ?? "", instance.QuantityInStock, instance.BuyingPrice, instance.SellingPrice, instance.Images?.FirstOrDefault()?.Path ?? "")));

        return new Result<GetProductQueryResponse>(productResponse);
    }

    public async Task<IResult<GetProductInstanceQueryResponse>> Handle(GetProductInstanceQuery request, CancellationToken cancellationToken)
    {
        var productInstance = await _productRepository.GetProductInstance(request.ProductId, request.ProductInstanceId);
        if (productInstance == null)
            return new Result<GetProductInstanceQueryResponse>().WithNotFound();

        var productInstanceResponse = new GetProductInstanceQueryResponse(
            productInstance.ProductId, 
            productInstance.Id, 
            productInstance.Sku ?? "", 
            productInstance.QuantityInStock,
            productInstance.BuyingPrice, 
            productInstance.SellingPrice, 
            productInstance.Images?.FirstOrDefault()?.Path,
            productInstance.ProductInstanceAttributeValues.Select(attribute => new Responses.ProductInstanceAttribute(attribute.AttributeId, attribute.AttributeValueId)));
        return new Result<GetProductInstanceQueryResponse>(productInstanceResponse);
    }

    public async Task<IResult<IEnumerable<GetProductsQueryResponse>>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _productRepository.GetProducts();
        return new Result<IEnumerable<GetProductsQueryResponse>>(products);
    }
}
