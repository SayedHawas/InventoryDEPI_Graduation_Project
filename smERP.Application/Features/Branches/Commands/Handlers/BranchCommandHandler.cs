using MediatR;
using smERP.Application.Contracts.Persistence;
using smERP.Application.Features.Branches.Commands.Models;
using smERP.Application.Features.StorageLocations.Commands.Models;
using smERP.Domain.Entities.Organization;
using smERP.SharedKernel.Localizations.Extensions;
using smERP.SharedKernel.Localizations.Resources;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.Branches.Commands.Handlers;

public class BranchCommandHandler(IBranchRepository branchRepository, IUnitOfWork unitOfWork) :
    IRequestHandler<AddBranchCommandModel, IResultBase>,
    IRequestHandler<EditBranchCommandModel, IResultBase>,
    IRequestHandler<DeleteBranchCommandModel, IResultBase>,
    IRequestHandler<AddStorageLocationCommandModel, IResultBase>,
    IRequestHandler<EditStorageLocationCommandModel, IResultBase>,
    IRequestHandler<DeleteStorageLocationCommandModel, IResultBase>

{
    private readonly IBranchRepository _branchRepository = branchRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<IResultBase> Handle(AddBranchCommandModel request, CancellationToken cancellationToken)
    {
        var doesEnglishNameExist = await _branchRepository.DoesExist(x => x.Name.English == request.EnglishName);
        if (doesEnglishNameExist)
            return new Result<Branch>()
                .WithBadRequest(SharedResourcesKeys.DoesExist.Localize(SharedResourcesKeys.NameEn.Localize()));

        var doesArabicNameExist = await _branchRepository.DoesExist(x => x.Name.Arabic == request.ArabicName);
        if (doesArabicNameExist)
            return new Result<Branch>()
                .WithBadRequest(SharedResourcesKeys.DoesExist.Localize(SharedResourcesKeys.NameAr.Localize()));

        var branchToBeCreatedResult = Branch.Create(request.EnglishName, request.ArabicName);
        if (branchToBeCreatedResult.IsFailed)
            return branchToBeCreatedResult;

        await _branchRepository.Add(branchToBeCreatedResult.Value, cancellationToken);

        await branchToBeCreatedResult
            .WithTask(() => _unitOfWork.SaveChangesAsync(cancellationToken), SharedResourcesKeys.DatabaseError);
        if (branchToBeCreatedResult.IsFailed)
            return branchToBeCreatedResult;

        return branchToBeCreatedResult.ChangeType(branchToBeCreatedResult.Value.Id).WithCreated();

    }

    public async Task<IResultBase> Handle(EditBranchCommandModel request, CancellationToken cancellationToken)
    {
        var branchToBeEdited = await _branchRepository.GetByID(request.BranchId);
        if (branchToBeEdited == null)
            return new Result<Branch>()
                .WithBadRequest(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.Branch.Localize()));

        if (!string.IsNullOrWhiteSpace(request.EnglishName))
        {
            var doesEnglishNameExist = await _branchRepository.DoesExist(x => x.Name.English == request.EnglishName && x.Id != request.BranchId);
            if (doesEnglishNameExist)
                return new Result<Branch>()
                    .WithBadRequest(SharedResourcesKeys.DoesExist.Localize(SharedResourcesKeys.NameEn.Localize()));

            branchToBeEdited.Name.UpdateEnglish(request.EnglishName);
        }

        if (!string.IsNullOrWhiteSpace(request.ArabicName))
        {
            var doesArabicNameExist = await _branchRepository.DoesExist(x => x.Name.Arabic == request.ArabicName && x.Id != request.BranchId);
            if (doesArabicNameExist)
                return new Result<Branch>()
                    .WithBadRequest(SharedResourcesKeys.DoesExist.Localize(SharedResourcesKeys.NameAr.Localize()));

            branchToBeEdited.Name.UpdateArabic(request.ArabicName);
        }

        _branchRepository.Update(branchToBeEdited);

        var branchUpdateResult = await new Result<Branch>(branchToBeEdited)
            .WithTask(() => _unitOfWork.SaveChangesAsync(cancellationToken), SharedResourcesKeys.DatabaseError);
        if (branchUpdateResult.IsFailed)
            return branchUpdateResult;

        return branchUpdateResult.WithUpdated();
    }

    public async Task<IResultBase> Handle(DeleteBranchCommandModel request, CancellationToken cancellationToken)
    {
        var branchToBeDeleted = await _branchRepository.GetByID(request.BranchId);
        if (branchToBeDeleted == null)
            return new Result<Branch>()
                .WithBadRequest(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.Branch.Localize()));

        _branchRepository.Remove(branchToBeDeleted);

        var branchDeleteResult = await new Result<Branch>(branchToBeDeleted)
            .WithTask(() => _unitOfWork.SaveChangesAsync(cancellationToken), SharedResourcesKeys.DatabaseError);
        if (branchDeleteResult.IsFailed)
            return branchDeleteResult;

        return branchDeleteResult.WithDeleted();
    }

    public async Task<IResultBase> Handle(AddStorageLocationCommandModel request, CancellationToken cancellationToken)
    {
        var branch = await _branchRepository.GetByIdWithStorageLocations(request.BranchId);
        if (branch == null)
            return new Result<StorageLocation>()
                .WithBadRequest(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.Branch.Localize()));

        var storageToBeCreatedResult = branch.AddStorageLocation(request.Name);
        if (storageToBeCreatedResult.IsFailed)
            return storageToBeCreatedResult;

        await storageToBeCreatedResult
            .WithTask(() => _unitOfWork.SaveChangesAsync(cancellationToken), SharedResourcesKeys.DatabaseError);
        if (storageToBeCreatedResult.IsFailed)
            return storageToBeCreatedResult;

        return storageToBeCreatedResult.ChangeType(storageToBeCreatedResult.Value.Id).WithCreated();
    }

    public async Task<IResultBase> Handle(EditStorageLocationCommandModel request, CancellationToken cancellationToken)
    {
        var branch = await _branchRepository.GetByIdWithStorageLocations(request.BranchId);
        if (branch == null)
            return new Result<StorageLocation>()
                .WithBadRequest(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.Branch.Localize()));

        var storageToBeUpdatedResult = branch.UpdateStorageLocation(request.StorageLocationId ,request.Name);
        if (storageToBeUpdatedResult.IsFailed)
            return storageToBeUpdatedResult;

        await storageToBeUpdatedResult
            .WithTask(() => _unitOfWork.SaveChangesAsync(cancellationToken), SharedResourcesKeys.DatabaseError);
        if (storageToBeUpdatedResult.IsFailed)
            return storageToBeUpdatedResult;

        return storageToBeUpdatedResult.WithUpdated();
    }

    public async Task<IResultBase> Handle(DeleteStorageLocationCommandModel request, CancellationToken cancellationToken)
    {
        var branch = await _branchRepository.GetByIdWithStorageLocations(request.BranchId);
        if (branch == null)
            return new Result<StorageLocation>()
                .WithBadRequest(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.Branch.Localize()));

        var storageToBeRemovedResult = branch.RemoveStorageLocation(request.StorageLocationId);
        if (storageToBeRemovedResult.IsFailed)
            return storageToBeRemovedResult;

        await storageToBeRemovedResult
            .WithTask(() => _unitOfWork.SaveChangesAsync(cancellationToken), SharedResourcesKeys.DatabaseError);
        if (storageToBeRemovedResult.IsFailed)
            return storageToBeRemovedResult;

        return storageToBeRemovedResult.WithDeleted();
    }
}