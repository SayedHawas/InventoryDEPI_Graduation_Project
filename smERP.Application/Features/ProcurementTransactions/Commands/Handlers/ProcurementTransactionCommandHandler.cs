using MediatR;
using smERP.Application.Contracts.Persistence;
using smERP.Application.Features.ProcurementTransactions.Commands.Models;
using smERP.Domain.Entities.InventoryTransaction;
using smERP.SharedKernel.Localizations.Extensions;
using smERP.SharedKernel.Localizations.Resources;
using smERP.SharedKernel.Responses;
using System.Transactions;

namespace smERP.Application.Features.ProcurementTransactions.Commands.Handlers;

public class ProcurementTransactionCommandHandler(
    IProcurementTransactionRepository procurementTransactionRepository,
    IBranchRepository branchRepository,
    IProductRepository productRepository,
    ISupplierRepository supplierRepository,
    IUnitOfWork unitOfWork,
    IMediator mediator) :
    IRequestHandler<AddProcurementTransactionCommandModel, IResultBase>,
    //IRequestHandler<EditProcurementTransactionCommandModelOld, IResultBase>,
    IRequestHandler<DeleteProcurementTransactionCommandModel, IResultBase>,
    IRequestHandler<EditProcurementTransactionCommandModel, IResultBase>,
    IRequestHandler<AddProcurementTransactionPaymentCommandModel, IResultBase>,
    IRequestHandler<EditProcurementTransactionPaymentCommandModel, IResultBase>,
    IRequestHandler<DeleteProcurementTransactionPaymentCommandModel, IResultBase>,
    IRequestHandler<AddProcurementTransactionProductCommandModel, IResultBase>,
    IRequestHandler<EditProcurementTransactionProductCommandModel, IResultBase>,
    IRequestHandler<DeleteProcurementTransactionProductCommandModel, IResultBase>
{
    private readonly IProcurementTransactionRepository _procurementTransactionRepository = procurementTransactionRepository;
    private readonly IBranchRepository _branchRepository = branchRepository;
    private readonly IProductRepository _productRepository = productRepository;
    private readonly ISupplierRepository _supplierRepository = supplierRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMediator _mediator = mediator;

    public async Task<IResultBase> Handle(AddProcurementTransactionCommandModel request, CancellationToken cancellationToken)
    {
        var branch = await _branchRepository.GetByIdWithStorageLocations(request.BranchId);
        if (branch == null)
            return new Result<ProcurementTransaction>()
                .WithBadRequest(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.Branch.Localize()));

        var storageLocation = branch.StorageLocations.FirstOrDefault(x => x.Id == request.StorageLocationId);
        if (storageLocation == null)
            return new Result<ProcurementTransaction>()
                .WithBadRequest(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.StorageLocation.Localize()));

        var supplier = await _supplierRepository.GetByID(request.SupplierId);
        if (supplier == null)
            return new Result<ProcurementTransaction>()
                .WithBadRequest(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.Supplier.Localize()));

        var productInstances = await _productRepository.GetProductInstancesWithProduct(request.Products.Select(x => x.ProductInstanceId));
        if (productInstances == null || productInstances.Count() != request.Products.Count)
            return new Result<ProcurementTransaction>()
                .WithBadRequest(SharedResourcesKeys.SomeItemsIn___ListAreNotCorrect.Localize(SharedResourcesKeys.Product.Localize()));

        var payments = new List<(decimal PayedAmount, string PaymentMethod)>();

        if (request.Payment != null)
        {
            payments.Add((request.Payment.PayedAmount, request.Payment.PaymentMethod));
        }

        var transactionItems = request.Products.Select(x =>
        {
            var productInstance = productInstances.FirstOrDefault(z => z.IsTracked && z.ProductInstanceId == x.ProductInstanceId);

            return (
                x.ProductInstanceId,
                x.Quantity,
                x.UnitPrice,
                productInstance.IsTracked,
                x.Units?.Select(item => item.SerialNumber).ToList()
            );
        }).ToList();

        var procurementTransactionToBeCreatedResult = ProcurementTransaction.Create(storageLocation.Id, request.SupplierId, payments, transactionItems);
        if (procurementTransactionToBeCreatedResult.IsFailed)
            return procurementTransactionToBeCreatedResult;

        await _procurementTransactionRepository.Add(procurementTransactionToBeCreatedResult.Value, cancellationToken);

        var productEntries = request.Products.Select(x =>
        {
            var productInstance = productInstances.FirstOrDefault(z => z.IsTracked && z.ProductInstanceId == x.ProductInstanceId);

            return (
                x.ProductInstanceId,
                x.Quantity,
                productInstance.IsTracked,
                productInstance.ShelfLifeInDays,
                x.Units?.Select(item => (item.SerialNumber, "Available", item.ExpirationDate)).ToList()
            );
        }).ToList();

        procurementTransactionToBeCreatedResult.Value.RaiseTransactionCreatedEvent(productEntries);

        foreach (var domainEvent in procurementTransactionToBeCreatedResult.Value.Events)
        {
            await _mediator.Publish(domainEvent, cancellationToken);
        }

        procurementTransactionToBeCreatedResult.Value.ClearEvents();

        await procurementTransactionToBeCreatedResult.WithTask(() => _unitOfWork.SaveChangesAsync(cancellationToken), SharedResourcesKeys.DatabaseError);
        if (procurementTransactionToBeCreatedResult.IsFailed)
            return procurementTransactionToBeCreatedResult;


        return procurementTransactionToBeCreatedResult.ChangeType(procurementTransactionToBeCreatedResult.Value.Id).WithCreated();
    }

    //public async Task<IResultBase> Handle(EditProcurementTransactionCommandModel request, CancellationToken cancellationToken)
    //{
    //    var procurementTransactionToBeEdited = await _procurementTransactionRepository.GetByID(request.ProcurementTransactionId);
    //    if (procurementTransactionToBeEdited == null)
    //        return new Result<ProcurementTransaction>()
    //            .WithBadRequest(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.ProcurementTransaction.Localize()));

    //    if (request.SupplierId != null && request.SupplierId.HasValue)
    //    {
    //        var doesSupplierExist = await _supplierRepository.DoesExist(request.SupplierId.Value);
    //        if (!doesSupplierExist)
    //            return new Result<ProcurementTransaction>()
    //                .WithBadRequest(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.Supplier.Localize()));

    //        procurementTransactionToBeEdited.UpdateSupplier(request.SupplierId.Value);
    //    }

    //    if (request.Products != null && request.Products.Count > 0)
    //    {
    //        var productInstances = await _productRepository.GetProductInstancesWithProduct(request.Products.Select(x => x.ProductInstanceId));
    //        if (productInstances == null || productInstances.Count() != request.Products.Count)
    //            return new Result<ProcurementTransaction>()
    //                .WithBadRequest(SharedResourcesKeys.SomeItemsIn___ListAreNotCorrect.Localize(SharedResourcesKeys.Product.Localize()));

    //        var transactionItemsToBeEdited = request.Products.Select(x =>
    //        {
    //            var productInstance = productInstances.FirstOrDefault(z => z.IsTracked && z.ProductInstanceId == x.ProductInstanceId);

    //            return (
    //                x.ProductInstanceId,
    //                x.Quantity,
    //                x.UnitPrice,
    //                productInstance.IsTracked,
    //                x.Items?.Select(item => item.SerialNumber).ToList()
    //            );
    //        }).ToList();

    //        //var ProductInstancesToBeEdited = request.Products.Select(x => (x.ProductInstanceId, x.Quantity, x.UnitPrice, x.Items?.Select(z => z.SerialNumber).ToList())).ToList();

    //        var procurementTransactionToBeEditedResult = procurementTransactionToBeEdited.UpdateTransactionItems(transactionItemsToBeEdited);
    //        if (procurementTransactionToBeEditedResult.IsFailed)
    //            return procurementTransactionToBeEditedResult;
    //    }

    //    if (request.Payments != null && request.Payments.Count > 0)
    //    {
    //        var ProductInstancesToBeEdited = request.Payments.Select(x => (x.PaymentTransactionId, x.PayedAmount, x.PaymentMethod)).ToList();

    //        var procurementTransactionToBeEditedResult = procurementTransactionToBeEdited.UpdateTransactionPayments(ProductInstancesToBeEdited);
    //        if (procurementTransactionToBeEditedResult.IsFailed)
    //            return procurementTransactionToBeEditedResult;
    //    }

    //    return new Result<ProcurementTransaction>().WithUpdated();
    //}

    public Task<IResultBase> Handle(DeleteProcurementTransactionCommandModel request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<IResultBase> Handle(EditProcurementTransactionCommandModel request, CancellationToken cancellationToken)
    {
        var procurementTransactionToBeEdited = await _procurementTransactionRepository.GetByID(request.TransactionId);
        if (procurementTransactionToBeEdited == null)
            return new Result<ProcurementTransaction>()
                .WithBadRequest(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.ProcurementTransaction.Localize()));

        if (request.SupplierId != null && request.SupplierId.HasValue)
        {
            var doesSupplierExist = await _supplierRepository.DoesExist(request.SupplierId.Value);
            if (!doesSupplierExist)
                return new Result<ProcurementTransaction>()
                    .WithBadRequest(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.Supplier.Localize()));

            procurementTransactionToBeEdited.UpdateSupplier(request.SupplierId.Value);
        }

        var productInstancesId = new[]
        {
            request.NewItems?.Select(x => x.ProductInstanceId) ?? [],
            request.ItemUpdates?.Select(x => x.ProductInstanceId) ?? [],
            request.ItemsToRemove ?? Enumerable.Empty<int>()
        }.SelectMany(x => x).Distinct().ToList();

        var productInstances = await _productRepository.GetProductInstancesWithProduct(productInstancesId);
        if (productInstances == null || productInstances.Count() != productInstancesId.Count)
            return new Result<ProcurementTransaction>()
                .WithBadRequest(SharedResourcesKeys.SomeItemsIn___ListAreNotCorrect.Localize(SharedResourcesKeys.Product.Localize()));

        if (request.ItemsToRemove != null && request.ItemsToRemove.Count > 0)
        {
            var removingItemsResult = procurementTransactionToBeEdited.RemoveItems(request.ItemsToRemove);
            if (removingItemsResult.IsFailed)
                return removingItemsResult;
        }

        if (request.ItemUpdates != null && request.ItemUpdates.Count > 0)
        {
            var itemUpdates = request.ItemUpdates.Select(x =>
            {
                var productInstance = productInstances.FirstOrDefault(z => z.IsTracked && z.ProductInstanceId == x.ProductInstanceId);

                return (
                    x.ProductInstanceId,
                    x.Quantity,
                    x.UnitPrice,
                    productInstance.IsTracked,
                    productInstance.ShelfLifeInDays,
                    x.UnitUpdates?.ToAdd?.Select(unitUpdate => (unitUpdate.SerialNumber, unitUpdate.ExpirationDate)).ToList(),
                    x.UnitUpdates?.ToRemove?.ToList()
                );
            }).ToList();

            var updatingItemsResult = procurementTransactionToBeEdited.UpdateItems(itemUpdates);
            if (updatingItemsResult.IsFailed)
                return updatingItemsResult;
        }

        if (request.NewItems != null && request.NewItems.Count > 0)
        {
            var newItems = request.NewItems.Select(x =>
            {
                var productInstance = productInstances.FirstOrDefault(z => z.IsTracked && z.ProductInstanceId == x.ProductInstanceId);

                return (
                    x.ProductInstanceId,
                    x.Quantity,
                    x.UnitPrice,
                    productInstance.IsTracked,
                    productInstance.ShelfLifeInDays,
                    x.Units?.Select(unit => (unit.SerialNumber, unit.ExpirationDate)).ToList()
                );
            }).ToList();

            var addingNewItemsResult = procurementTransactionToBeEdited.AddItems(newItems);
            if (addingNewItemsResult.IsFailed)
                return addingNewItemsResult;
        }

        if (request.PaymentsToRemove != null && request.PaymentsToRemove.Count > 0)
        {
            var paymentsRemovingResult = procurementTransactionToBeEdited.RemoveTransactionPayments(request.PaymentsToRemove);
            if (paymentsRemovingResult.IsFailed)
                return paymentsRemovingResult;
        }

        if (request.PaymentUpdates != null && request.PaymentUpdates.Count > 0)
        {
            var paymentsUpdatingResult = procurementTransactionToBeEdited.UpdateTransactionPayments(request.PaymentUpdates.Select(payment => (payment.PaymentTransactionId, payment.PayedAmount, payment.PaymentMethod)).ToList());
            if (paymentsUpdatingResult.IsFailed)
                return paymentsUpdatingResult;
        }

        if (request.NewPayments != null && request.NewPayments.Count > 0)
        {
            var paymentsAddResult = procurementTransactionToBeEdited.AddTransactionPayments(request.NewPayments.Select(payment => (payment.PayedAmount, payment.PaymentMethod)).ToList());
            if (paymentsAddResult.IsFailed)
                return paymentsAddResult;
        }

        _procurementTransactionRepository.Update(procurementTransactionToBeEdited);

        foreach (var domainEvent in procurementTransactionToBeEdited.Events)
        {
            await _mediator.Publish(domainEvent, cancellationToken);
        }

        procurementTransactionToBeEdited.ClearEvents();

        var savingProcurementTransactionUpdatesResult = await new Result<ProcurementTransaction>().WithTask(() => _unitOfWork.SaveChangesAsync(cancellationToken), SharedResourcesKeys.DatabaseError);
        if (savingProcurementTransactionUpdatesResult.IsFailed)
            return savingProcurementTransactionUpdatesResult;

        return savingProcurementTransactionUpdatesResult.WithUpdated();
    }

    public async Task<IResultBase> Handle(AddProcurementTransactionPaymentCommandModel request, CancellationToken cancellationToken)
    {
        var transaction = await _procurementTransactionRepository.GetByID(request.TransactionId);
        if (transaction == null)
            return new Result<ProcurementTransaction>().WithNotFound(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.ProcurementTransaction.Localize()));

        var addPaymentResult = transaction.AddTransactionPayments([(request.PayedAmount, request.PaymentMethod)]);
        if (addPaymentResult.IsFailed)
            return addPaymentResult;

        _procurementTransactionRepository.Update(transaction);

        foreach (var domainEvent in transaction.Events)
        {
            await _mediator.Publish(domainEvent, cancellationToken);
        }

        transaction.ClearEvents();

        await addPaymentResult.WithTask(() => _unitOfWork.SaveChangesAsync(cancellationToken), SharedResourcesKeys.DatabaseError);
        if (addPaymentResult.IsFailed)
            return addPaymentResult;

        return addPaymentResult.WithCreated();
    }

    public async Task<IResultBase> Handle(EditProcurementTransactionPaymentCommandModel request, CancellationToken cancellationToken)
    {
        var transaction = await _procurementTransactionRepository.GetByID(request.TransactionId);
        if (transaction == null)
            return new Result<ProcurementTransaction>().WithNotFound(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.ProcurementTransaction.Localize()));

        var editPaymentResult = transaction.UpdateTransactionPayments([(request.PaymentId, request.PayedAmount, request.PaymentMethod)]);
        if (editPaymentResult.IsFailed)
            return editPaymentResult;

        _procurementTransactionRepository.Update(transaction);


        foreach (var domainEvent in transaction.Events)
        {
            await _mediator.Publish(domainEvent, cancellationToken);
        }

        transaction.ClearEvents();

        await editPaymentResult.WithTask(() => _unitOfWork.SaveChangesAsync(cancellationToken), SharedResourcesKeys.DatabaseError);
        if (editPaymentResult.IsFailed)
            return editPaymentResult;

        return editPaymentResult.WithUpdated();
    }

    public async Task<IResultBase> Handle(DeleteProcurementTransactionPaymentCommandModel request, CancellationToken cancellationToken)
    {
        var transaction = await _procurementTransactionRepository.GetByID(request.TransactionId);
        if (transaction == null)
            return new Result<ProcurementTransaction>().WithNotFound(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.ProcurementTransaction.Localize()));

        var removePaymentResult = transaction.RemoveTransactionPayments([request.PaymentId]);
        if (removePaymentResult.IsFailed)
            return removePaymentResult;

        _procurementTransactionRepository.Update(transaction);


        foreach (var domainEvent in transaction.Events)
        {
            await _mediator.Publish(domainEvent, cancellationToken);
        }

        transaction.ClearEvents();

        await removePaymentResult.WithTask(() => _unitOfWork.SaveChangesAsync(cancellationToken), SharedResourcesKeys.DatabaseError);
        if (removePaymentResult.IsFailed)
            return removePaymentResult;

        return removePaymentResult.WithDeleted();
    }

    public async Task<IResultBase> Handle(AddProcurementTransactionProductCommandModel request, CancellationToken cancellationToken)
    {
        var transaction = await _procurementTransactionRepository.GetByID(request.TransactionId);
        if (transaction == null)
            return new Result<ProcurementTransaction>().WithNotFound(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.ProcurementTransaction.Localize()));

        var productInstance = await _productRepository.GetProductInstance(request.ProductInstanceId);
        if (productInstance == null)
            return new Result<ProcurementTransaction>().WithNotFound(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.Product.Localize()));

        var productToAdd = (productInstance.ProductInstanceId, request.Quantity, request.UnitPrice, productInstance.IsTracked, productInstance.ShelfLifeInDays,
            request?.UnitsToAdd?.Select(unit => (unit.SerialNumber, unit.ExpirationDate)).ToList());

        var addProductResult = transaction.AddItems([productToAdd]);
        if (addProductResult.IsFailed)
            return addProductResult;

        _procurementTransactionRepository.Update(transaction);


        foreach (var domainEvent in transaction.Events)
        {
            await _mediator.Publish(domainEvent, cancellationToken);
        }

        transaction.ClearEvents();

        await addProductResult.WithTask(() => _unitOfWork.SaveChangesAsync(cancellationToken), SharedResourcesKeys.DatabaseError);
        if (addProductResult.IsFailed)
            return addProductResult;

        return addProductResult.WithCreated();
    }

    public async Task<IResultBase> Handle(EditProcurementTransactionProductCommandModel request, CancellationToken cancellationToken)
    {
        var transaction = await _procurementTransactionRepository.GetByID(request.TransactionId);
        if (transaction == null)
            return new Result<ProcurementTransaction>().WithNotFound(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.ProcurementTransaction.Localize()));

        var productInstance = await _productRepository.GetProductInstance(request.ProductInstanceId);
        if (productInstance == null)
            return new Result<ProcurementTransaction>().WithNotFound(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.Product.Localize()));

        var productToEdit = (productInstance.ProductInstanceId, request.Quantity, request.UnitPrice, productInstance.IsTracked, productInstance.ShelfLifeInDays,
            request.UnitsToAdd?.Select(unit => (unit.SerialNumber, unit.ExpirationDate)).ToList(),
            request.UnitsToRemove?.Select(unit => unit).ToList());

        var editProductResult = transaction.UpdateItems([productToEdit]);
        if (editProductResult.IsFailed)
            return editProductResult;

        _procurementTransactionRepository.Update(transaction);


        foreach (var domainEvent in transaction.Events)
        {
            await _mediator.Publish(domainEvent, cancellationToken);
        }

        transaction.ClearEvents();

        await editProductResult.WithTask(() => _unitOfWork.SaveChangesAsync(cancellationToken), SharedResourcesKeys.DatabaseError);
        if (editProductResult.IsFailed)
            return editProductResult;

        return editProductResult.WithUpdated();
    }

    public async Task<IResultBase> Handle(DeleteProcurementTransactionProductCommandModel request, CancellationToken cancellationToken)
    {
        var transaction = await _procurementTransactionRepository.GetByID(request.TransactionId);
        if (transaction == null)
            return new Result<ProcurementTransaction>().WithNotFound(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.ProcurementTransaction.Localize()));

        var doesProductInstanceExist = await _productRepository.DoesProductInstanceExist(request.ProductInstanceId);
        if (!doesProductInstanceExist)
            return new Result<ProcurementTransaction>().WithNotFound(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.Product.Localize()));

        var removeProductResult = transaction.RemoveTransactionItems([request.ProductInstanceId]);
        if (removeProductResult.IsFailed)
            return removeProductResult;


        foreach (var domainEvent in transaction.Events)
        {
            await _mediator.Publish(domainEvent, cancellationToken);
        }

        transaction.ClearEvents();

        _procurementTransactionRepository.Update(transaction);

        await removeProductResult.WithTask(() => _unitOfWork.SaveChangesAsync(cancellationToken), SharedResourcesKeys.DatabaseError);
        if (removeProductResult.IsFailed)
            return removeProductResult;

        return removeProductResult.WithDeleted();
    }
}