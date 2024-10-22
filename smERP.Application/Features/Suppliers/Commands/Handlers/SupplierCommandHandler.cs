using MediatR;
using smERP.Application.Contracts.Persistence;
using smERP.Application.Features.Suppliers.Commands.Models;
using smERP.Domain.Entities.ExternalEntities;
using smERP.Domain.Entities.Product;
using smERP.SharedKernel.Localizations.Extensions;
using smERP.SharedKernel.Localizations.Resources;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.Suppliers.Commands.Handlers;

public class SupplierCommandHandler(ISupplierRepository supplierRepository, IUnitOfWork unitOfWork) :
    IRequestHandler<AddSupplierCommandModel, IResultBase>,
    IRequestHandler<EditSupplierCommandModel, IResultBase>,
    IRequestHandler<DeleteSupplierCommandModel, IResultBase>
{
    private readonly ISupplierRepository _supplierRepository = supplierRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<IResultBase> Handle(AddSupplierCommandModel request, CancellationToken cancellationToken)
    {
        var doesEnglishNameExist = await _supplierRepository.DoesExist(x => x.Name.English == request.EnglishName);
        if (doesEnglishNameExist)
            return new Result<Supplier>()
                .WithBadRequest(SharedResourcesKeys.DoesExist.Localize(SharedResourcesKeys.NameEn.Localize()));

        var doesArabicNameExist = await _supplierRepository.DoesExist(x => x.Name.Arabic == request.ArabicName);
        if (doesArabicNameExist)
            return new Result<Supplier>()
                .WithBadRequest(SharedResourcesKeys.DoesExist.Localize(SharedResourcesKeys.NameAr.Localize()));

        var addressesList = request.Addresses.Select(ad => (ad.Street, ad.City, ad.State, ad.Country, ad.PostalCode, ad.Comment)).ToList();

        var supplierToBeCreatedResult = Supplier.Create(request.EnglishName, request.ArabicName, addressesList);
        if (supplierToBeCreatedResult.IsFailed)
            return supplierToBeCreatedResult;

        await _supplierRepository.Add(supplierToBeCreatedResult.Value, cancellationToken);

        await supplierToBeCreatedResult.WithTask(() => _unitOfWork.SaveChangesAsync(cancellationToken), SharedResourcesKeys.DatabaseError);
        if (supplierToBeCreatedResult.IsFailed)
            return supplierToBeCreatedResult;

        return supplierToBeCreatedResult.ChangeType(supplierToBeCreatedResult.Value.Id).WithCreated();
    }

    public async Task<IResultBase> Handle(EditSupplierCommandModel request, CancellationToken cancellationToken)
    {
        var supplierToBeEdited = await _supplierRepository.GetByID(request.SupplierId);
        if (supplierToBeEdited == null)
            return new Result<Supplier>()
                .WithBadRequest(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.Supplier.Localize()));

        var supplierToBeEditedResult = new Result<Supplier>(supplierToBeEdited);

        if (!string.IsNullOrEmpty(request.EnglishName))
        {
            var doesEnglishNameExist = await _supplierRepository.DoesExist(x => x.Name.English == request.EnglishName && x.Id != request.SupplierId);
            if (doesEnglishNameExist)
                return new Result<Supplier>()
                    .WithBadRequest(SharedResourcesKeys.DoesExist.Localize(SharedResourcesKeys.NameEn.Localize()));

            var englishNameEditResult = supplierToBeEdited.Name.UpdateEnglish(request.EnglishName);
            if (englishNameEditResult.IsFailed)
                return englishNameEditResult;
        }

        if (!string.IsNullOrEmpty(request.ArabicName))
        {
            var doesArabicNameExist = await _supplierRepository.DoesExist(x => x.Name.Arabic == request.ArabicName && x.Id != request.SupplierId);
            if (doesArabicNameExist)
                return new Result<Supplier>()
                    .WithBadRequest(SharedResourcesKeys.DoesExist.Localize(SharedResourcesKeys.NameAr.Localize()));

            var arabicNameEditResult = supplierToBeEdited.Name.UpdateArabic(request.ArabicName);
            if (arabicNameEditResult.IsFailed)
                return arabicNameEditResult;
        }

        if (request.Addresses != null && request.Addresses.Count > 0)
        {
            var addressesList = request.Addresses.Select(ad => (ad.Street, ad.City, ad.State, ad.Country, ad.PostalCode, ad.Comment)).ToList();
            var addressesUpdateResult = supplierToBeEdited.UpdateAddresses(addressesList);
            if (addressesUpdateResult.IsFailed)
                return addressesUpdateResult;
        }

        _supplierRepository.Update(supplierToBeEdited);

        await supplierToBeEditedResult.WithTask(() => _unitOfWork.SaveChangesAsync(cancellationToken), SharedResourcesKeys.DatabaseError);
        if (supplierToBeEditedResult.IsFailed)
            return supplierToBeEditedResult;

        return supplierToBeEditedResult.WithUpdated();
    }

    public async Task<IResultBase> Handle(DeleteSupplierCommandModel request, CancellationToken cancellationToken)
    {
        var supplierToBeDeleted = await _supplierRepository.GetByID(request.SupplierId);
        if (supplierToBeDeleted == null)
            return new Result<Supplier>()
                .WithBadRequest(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.Supplier.Localize()));

        _supplierRepository.Remove(supplierToBeDeleted);

        var supplierToBeDeletedResult = await new Result<Supplier>(supplierToBeDeleted).WithTask(() => _unitOfWork.SaveChangesAsync(cancellationToken), SharedResourcesKeys.DatabaseError);
        if (supplierToBeDeletedResult.IsFailed)
            return supplierToBeDeletedResult;

        return supplierToBeDeletedResult.WithDeleted();
    }
}
