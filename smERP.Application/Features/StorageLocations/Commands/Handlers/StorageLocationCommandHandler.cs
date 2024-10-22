using MediatR;
using smERP.Application.Contracts.Persistence;
using smERP.Application.Features.ProcurementTransactions.Commands.Models;
using smERP.Application.Features.StorageLocations.Commands.Models;
using smERP.Domain.Entities.InventoryTransaction;
using smERP.SharedKernel.Localizations.Extensions;
using smERP.SharedKernel.Localizations.Resources;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.StorageLocations.Commands.Handlers;

public class StorageLocationCommandHandler(
    IStorageLocationRepository storageLocationRepository,
    IProcurementTransactionRepository procurementTransactionRepository,
    IProductRepository productRepository,
    IUnitOfWork unitOfWork) :
    IRequestHandler<AddProductInstanceToStorageLocationModel, IResultBase>,
    IRequestHandler<EditProductInstanceToStorageLocationModel, IResultBase>
{
    private readonly IStorageLocationRepository _storageLocationRepository = storageLocationRepository;
    private readonly IProcurementTransactionRepository _procurementTransactionRepository = procurementTransactionRepository;
    private readonly IProductRepository _productRepository = productRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<IResultBase> Handle(AddProductInstanceToStorageLocationModel request, CancellationToken cancellationToken)
    {
        var storageLocation = await _storageLocationRepository.GetByID(request.StorageLocationId);
        if (storageLocation == null)
            return new Result<ProcurementTransaction>()
                .WithBadRequest(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.StorageLocation.Localize()));

        var procurementTransaction = await _procurementTransactionRepository.GetByID(request.ProcurementTransactionId);
        if (procurementTransaction == null)
            return new Result<ProcurementTransaction>()
                .WithBadRequest(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.ProcurementTransaction.Localize()));

        if (procurementTransaction.IsTransactionProcessed)
            return new Result<ProcurementTransaction>()
                .WithBadRequest(SharedResourcesKeys.ThisTransactionIsAlreadyProcessed.Localize());

        var procurementItems = procurementTransaction.Items
            .Select(x => new { x.ProductInstanceId, x.Quantity })
            .ToHashSet();

        var requestProducts = request.Products
            .Select(x => new { x.ProductInstanceId, x.Quantity })
            .ToHashSet();

        if (!procurementItems.SetEquals(requestProducts))
            return new Result<ProcurementTransaction>()
                .WithBadRequest(SharedResourcesKeys.SomeItemsIn___ListAreNotCorrect.Localize(SharedResourcesKeys.Product.Localize()));
        
        var productInstances = await _productRepository.GetProductInstancesWithProduct(request.Products.Select(x => x.ProductInstanceId));
        if (productInstances == null || productInstances.Count() != request.Products.Count)
            return new Result<ProcurementTransaction>()
                .WithBadRequest(SharedResourcesKeys.SomeItemsIn___ListAreNotCorrect.Localize(SharedResourcesKeys.Product.Localize()));

        var productToBeStored = request.Products.Select(x =>
        {
            var productInstance = productInstances.FirstOrDefault(z => z.IsTracked && z.ProductInstanceId == x.ProductInstanceId);

            return (
                x.ProductInstanceId,
                x.Quantity,
                productInstance.IsTracked,
                productInstance.ShelfLifeInDays,
                (x.Units ?? Enumerable.Empty<ProductItem>()).Select(m => (m.SerialNumber, "Available", m.ExpirationDate)).ToList()
            );
        }).ToList();

        var productToBeStoredAddResult = storageLocation.AddStoredProductInstances(productToBeStored);
        if (productToBeStoredAddResult.IsFailed)
            return productToBeStoredAddResult;

        _storageLocationRepository.Update(storageLocation);

        await productToBeStoredAddResult.WithTask(() => _unitOfWork.SaveChangesAsync(cancellationToken), SharedResourcesKeys.DatabaseError);
        if (productToBeStoredAddResult.IsFailed)
            return productToBeStoredAddResult;

        return new Result<ProcurementTransaction>().WithCreated();
    }

    public async Task<IResultBase> Handle(EditProductInstanceToStorageLocationModel request, CancellationToken cancellationToken)
    {
        var storageLocation = await _storageLocationRepository.GetByID(request.StorageLocationId);
        if (storageLocation == null)
            return new Result<ProcurementTransaction>()
                .WithBadRequest(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.StorageLocation.Localize()));

        var procurementTransaction = await _procurementTransactionRepository.GetByID(request.ProcurementTransactionId);
        if (procurementTransaction == null)
            return new Result<ProcurementTransaction>()
                .WithBadRequest(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.ProcurementTransaction.Localize()));

        if (procurementTransaction.IsTransactionProcessed)
            return new Result<ProcurementTransaction>()
                .WithBadRequest(SharedResourcesKeys.ThisTransactionIsAlreadyProcessed.Localize());

        var procurementItems = procurementTransaction.Items
            .Select(x => new { x.ProductInstanceId, x.Quantity })
            .ToHashSet();

        var requestProducts = request.Products
            .Select(x => new { x.ProductInstanceId, x.Quantity })
            .ToHashSet();

        if (!procurementItems.SetEquals(requestProducts))
            return new Result<ProcurementTransaction>()
                .WithBadRequest(SharedResourcesKeys.SomeItemsIn___ListAreNotCorrect.Localize(SharedResourcesKeys.Product.Localize()));

        var productInstances = await _productRepository.GetProductInstancesWithProduct(request.Products.Select(x => x.ProductInstanceId));
        if (productInstances == null || productInstances.Count() != request.Products.Count)
            return new Result<ProcurementTransaction>()
                .WithBadRequest(SharedResourcesKeys.SomeItemsIn___ListAreNotCorrect.Localize(SharedResourcesKeys.Product.Localize()));

        var productToBeStored = request.Products.Select(x =>
        {
            var productInstance = productInstances.FirstOrDefault(z => z.IsTracked && z.ProductInstanceId == x.ProductInstanceId);

            return (
                x.ProductInstanceId,
                x.Quantity,
                productInstance.IsTracked,
                productInstance.ShelfLifeInDays,
                (x.Units ?? Enumerable.Empty<ProductItem>()).Select(m => (m.SerialNumber, "Available", m.ExpirationDate)).ToList()
            );
        }).ToList();

        var productToBeStoredAddResult = storageLocation.AddStoredProductInstances(productToBeStored);
        if (productToBeStoredAddResult.IsFailed)
            return productToBeStoredAddResult;

        _storageLocationRepository.Update(storageLocation);

        return productToBeStoredAddResult;
    }
}