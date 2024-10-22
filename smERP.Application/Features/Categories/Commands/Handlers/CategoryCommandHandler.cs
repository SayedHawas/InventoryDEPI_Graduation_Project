using MediatR;
using smERP.Application.Contracts.Persistence;
using smERP.Application.Features.Categories.Commands.Models;
using smERP.Domain.Entities.Product;
using smERP.SharedKernel.Localizations.Extensions;
using smERP.SharedKernel.Localizations.Resources;
using smERP.SharedKernel.Responses;
using System.Net;

namespace smERP.Application.Features.Categories.Commands.Handlers;

public class CategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork) :
    IRequestHandler<AddCategoryCommandModel, IResultBase>,
    IRequestHandler<EditCategoryCommandModel, IResultBase>,
    IRequestHandler<DeleteCategoryCommandModel, IResultBase>
{
    private readonly ICategoryRepository _categoryRepository = categoryRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<IResultBase> Handle(AddCategoryCommandModel request, CancellationToken cancellationToken)
    {
        if (request.ParentCategoryId.HasValue && request.ParentCategoryId.Value > 0)
        {
            var parentCategory = await _categoryRepository.GetByID(request.ParentCategoryId.Value);
            if (parentCategory == null)
                return new Result<Category>()
                    .WithBadRequest(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.ParentCategory.Localize()));

            if (parentCategory.ProductCount != 0 || !parentCategory.IsLeaf)
                return new Result<Category>()
                    .WithBadRequest(SharedResourcesKeys.ParentCategoryMustHaveNoProduct.Localize());
        }

        var doesEnglishNameExist = await _categoryRepository.DoesExist(x => x.Name.English == request.EnglishName);
        if (doesEnglishNameExist)
            return new Result<Category>()
                .WithBadRequest(SharedResourcesKeys.DoesExist.Localize(SharedResourcesKeys.NameEn.Localize()));

        var doesArabicNameExist = await _categoryRepository.DoesExist(x => x.Name.Arabic == request.ArabicName);
        if (doesArabicNameExist)
            return new Result<Category>()
                .WithBadRequest(SharedResourcesKeys.DoesExist.Localize(SharedResourcesKeys.NameAr.Localize()));

        var categoryToBeCreated = Category.Create(request.EnglishName, request.ArabicName, request.ParentCategoryId);
        if (categoryToBeCreated.IsFailed)
            return categoryToBeCreated;

        await categoryToBeCreated.WithTask(() => _categoryRepository.Add(categoryToBeCreated.Value, cancellationToken), SharedResourcesKeys.DatabaseError);
        if (categoryToBeCreated.IsFailed)
            return categoryToBeCreated;

        await categoryToBeCreated.WithTask(() => _unitOfWork.SaveChangesAsync(cancellationToken), SharedResourcesKeys.DatabaseError);
        if (categoryToBeCreated.IsFailed)
            return categoryToBeCreated;

        var result = categoryToBeCreated.ChangeType(categoryToBeCreated.Value.Id)
                        .WithCreated();
        return result;
    }

    public async Task<IResultBase> Handle(EditCategoryCommandModel request, CancellationToken cancellationToken)
    {
        var categoryToBeEdited = await _categoryRepository.GetByID(request.CategoryId);
        if (categoryToBeEdited == null)
            return new Result<Category>()
                .WithBadRequest(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.Category.Localize()));

        if (request.ParentCategoryId.HasValue && request.ParentCategoryId.Value > 0)
        {
            var doesParentCategoryExist = await _categoryRepository.DoesExist(request.ParentCategoryId.Value);
            if (!doesParentCategoryExist)
                return new Result<Category>()
                    .WithBadRequest(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.ParentCategory.Localize()));

            categoryToBeEdited.UpdateParentCategory(request.ParentCategoryId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.EnglishName))
        {
            var doesEnglishNameExist = await _categoryRepository.DoesExist(x => x.Name.English == request.EnglishName && x.Id != request.CategoryId);
            if (doesEnglishNameExist)
                return new Result<Category>()
                    .WithBadRequest(SharedResourcesKeys.DoesExist.Localize(SharedResourcesKeys.NameEn.Localize()));

            categoryToBeEdited.Name.UpdateEnglish(request.EnglishName);
        }

        if (!string.IsNullOrWhiteSpace(request.ArabicName))
        {
            var doesArabicNameExist = await _categoryRepository.DoesExist(x => x.Name.Arabic == request.ArabicName && x.Id != request.CategoryId);
            if (doesArabicNameExist)
                return new Result<Category>()
                    .WithBadRequest(SharedResourcesKeys.DoesExist.Localize(SharedResourcesKeys.NameAr.Localize()));

            categoryToBeEdited.Name.UpdateArabic(request.ArabicName);
        }

        _categoryRepository.Update(categoryToBeEdited);

        var categoryUpdateResult = await new Result<Category>()
            .WithTask(() => _unitOfWork.SaveChangesAsync(cancellationToken), SharedResourcesKeys.DatabaseError.Localize());
        if (categoryUpdateResult.IsFailed)
            return categoryUpdateResult;

        return categoryUpdateResult.WithUpdated();
    }

    public async Task<IResultBase> Handle(DeleteCategoryCommandModel request, CancellationToken cancellationToken)
    {
        var categoryToBeDeleted = await _categoryRepository.GetByID(request.CategoryID);
        if (categoryToBeDeleted == null)
            return new Result<Category>()
                    .WithBadRequest(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.Category.Localize()));

        _categoryRepository.Remove(categoryToBeDeleted);

        var categoryDeleteResult = await new Result<Category>()
            .WithTask(() => _unitOfWork.SaveChangesAsync(cancellationToken), SharedResourcesKeys.DatabaseError.Localize());
        if (categoryDeleteResult.IsFailed)
            return categoryDeleteResult;

        return categoryDeleteResult.WithDeleted();
    }
}
