using MediatR;
using smERP.Application.Contracts.Persistence;
using smERP.Application.Features.Brands.Commands.Models;
using smERP.Application.Features.Categories.Commands.Models;
using smERP.Domain.Entities.Product;
using smERP.SharedKernel.Localizations.Extensions;
using smERP.SharedKernel.Localizations.Resources;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.Brands.Commands.Handlers;

public class BrandCommandHandler (IBrandRepository brandRepository, IUnitOfWork unitOfWork) :
    IRequestHandler<AddBrandCommandModel, IResultBase>,
    IRequestHandler<EditBrandCommandModel, IResultBase>,
    IRequestHandler<DeleteBrandCommandModel, IResultBase>
{
    private readonly IBrandRepository _brandRepository = brandRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<IResultBase> Handle(AddBrandCommandModel request, CancellationToken cancellationToken)
    {
        var doesEnglishNameExist = await _brandRepository.DoesExist(x => x.Name.English == request.EnglishName);
        if (doesEnglishNameExist)
            return new Result<Brand>()
                .WithBadRequest(SharedResourcesKeys.DoesExist.Localize(SharedResourcesKeys.NameEn.Localize()));

        var doesArabicNameExist = await _brandRepository.DoesExist(x => x.Name.Arabic == request.ArabicName);
        if (doesArabicNameExist)
            return new Result<Brand>()
                .WithBadRequest(SharedResourcesKeys.DoesExist.Localize(SharedResourcesKeys.NameAr.Localize()));

        var brandToBeCreated = Brand.Create(request.EnglishName, request.ArabicName);
        if (brandToBeCreated.IsFailed)
            return brandToBeCreated;

        await brandToBeCreated.WithTask(() => _brandRepository.Add(brandToBeCreated.Value, cancellationToken), SharedResourcesKeys.DatabaseError);
        if (brandToBeCreated.IsFailed)
            return brandToBeCreated;

        await brandToBeCreated.WithTask(() => _unitOfWork.SaveChangesAsync(cancellationToken), SharedResourcesKeys.DatabaseError);
        if (brandToBeCreated.IsFailed)
            return brandToBeCreated;

        var result = brandToBeCreated.ChangeType(brandToBeCreated.Value.Id)
                        .WithCreated();
        return result;
    }

    public async Task<IResultBase> Handle(EditBrandCommandModel request, CancellationToken cancellationToken)
    {
        var brandToBeEdited = await _brandRepository.GetByID(request.BrandId);
        if (brandToBeEdited == null)
            return new Result<Brand>()
                .WithBadRequest(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.Brand.Localize()));

        if (!string.IsNullOrWhiteSpace(request.EnglishName))
        {
            var doesEnglishNameExist = await _brandRepository.DoesExist(x => x.Name.English == request.EnglishName && x.Id != request.BrandId);
            if (doesEnglishNameExist)
                return new Result<Brand>()
                    .WithBadRequest(SharedResourcesKeys.DoesExist.Localize(SharedResourcesKeys.NameEn.Localize()));

            brandToBeEdited.Name.UpdateEnglish(request.EnglishName);
        }

        if (!string.IsNullOrWhiteSpace(request.ArabicName))
        {
            var doesArabicNameExist = await _brandRepository.DoesExist(x => x.Name.Arabic == request.ArabicName && x.Id != request.BrandId);
            if (doesArabicNameExist)
                return new Result<Brand>()
                    .WithBadRequest(SharedResourcesKeys.DoesExist.Localize(SharedResourcesKeys.NameAr.Localize()));

            brandToBeEdited.Name.UpdateArabic(request.ArabicName);
        }

        _brandRepository.Update(brandToBeEdited);

        var brandUpdateResult = await new Result<Brand>()
            .WithTask(() => _unitOfWork.SaveChangesAsync(cancellationToken), SharedResourcesKeys.DatabaseError.Localize());
        if (brandUpdateResult.IsFailed)
            return brandUpdateResult;

        return brandUpdateResult.WithUpdated();
    }

    public async Task<IResultBase> Handle(DeleteBrandCommandModel request, CancellationToken cancellationToken)
    {
        var brandToBeDeleted = await _brandRepository.GetByID(request.BrandId);
        if (brandToBeDeleted == null)
            return new Result<Brand>()
                    .WithBadRequest(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.Brand.Localize()));

        _brandRepository.Remove(brandToBeDeleted);

        var brandDeleteResult = await new Result<Brand>()
            .WithTask(() => _unitOfWork.SaveChangesAsync(cancellationToken), SharedResourcesKeys.DatabaseError.Localize());
        if (brandDeleteResult.IsFailed)
            return brandDeleteResult;

        return brandDeleteResult.WithDeleted();
    }
}