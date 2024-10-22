using MediatR;
using smERP.Application.Contracts.Persistence;
using smERP.Application.Features.Attributes.Commands.Models;
using smERP.Application.Features.Products.Commands.Models;
using smERP.Domain.Entities.Product;
using smERP.SharedKernel.Localizations.Extensions;
using smERP.SharedKernel.Localizations.Resources;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.Products.Commands.Handlers;

public class ProductCommandHandler(
    IProductRepository productRepository,
    ICategoryRepository categoryRepository, 
    IBrandRepository brandRepository, 
    IUnitOfWork unitOfWork) :
    IRequestHandler<AddProductCommandModel, IResultBase>,
    IRequestHandler<EditProductCommandModel, IResultBase>,
    IRequestHandler<DeleteProductCommandModel, IResultBase>
{
    private readonly IProductRepository _productRepository = productRepository;
    private readonly ICategoryRepository _categoryRepository = categoryRepository;
    private readonly IBrandRepository _brandRepository = brandRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<IResultBase> Handle(AddProductCommandModel request, CancellationToken cancellationToken)
    {
        var doesEnglishNameExist = await _productRepository.DoesExist(x => x.Name.English == request.EnglishName);
        if (doesEnglishNameExist)
            return new Result<Product>()
                .WithBadRequest(SharedResourcesKeys.DoesExist.Localize(SharedResourcesKeys.NameEn.Localize()));

        var doesArabicNameExist = await _productRepository.DoesExist(x => x.Name.Arabic == request.ArabicName);
        if (doesArabicNameExist)
            return new Result<Product>()
                .WithBadRequest(SharedResourcesKeys.DoesExist.Localize(SharedResourcesKeys.NameAr.Localize()));

        var doesBrandExist = await _brandRepository.DoesExist(x => x.Id == request.BrandId);
        if (!doesBrandExist)
            return new Result<Product>()
                .WithBadRequest(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.Brand.Localize()));

        var doesCategoryExist = await _categoryRepository.DoesExist(x => x.Id == request.CategoryId);
        if (!doesCategoryExist)
            return new Result<Product>()
                .WithBadRequest(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.Category.Localize()));

        var doesModelNumberExist = await _productRepository.DoesExist(x => x.ModelNumber == request.ModelNumber);
        if (doesModelNumberExist)
            return new Result<Product>()
                .WithBadRequest(SharedResourcesKeys.DoesExist.Localize(SharedResourcesKeys.ModelNumber.Localize()));

        var productToBeCreatedResult = Product.Create(request.EnglishName, request.ArabicName, request.ModelNumber, request.BrandId, request.CategoryId, request.Description, request.ShelfLifeInDays);
        if (productToBeCreatedResult.IsFailed)
            return productToBeCreatedResult;

        await _productRepository.Add(productToBeCreatedResult.Value, cancellationToken);

        await productToBeCreatedResult
            .WithTask(() => _unitOfWork.SaveChangesAsync(cancellationToken), SharedResourcesKeys.DatabaseError);
        if (productToBeCreatedResult.IsFailed)
            return productToBeCreatedResult;

        return productToBeCreatedResult.ChangeType(productToBeCreatedResult.Value.Id).WithCreated();
    }

    public async Task<IResultBase> Handle(EditProductCommandModel request, CancellationToken cancellationToken)
    {
        var productToBeEdited = await _productRepository.GetByID(request.ProductId);
        if (productToBeEdited == null)
            return new Result<Product>()
                .WithBadRequest(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.Product.Localize()));

        if (!string.IsNullOrEmpty(request.EnglishName))
        {
            var doesEnglishNameExist = await _productRepository.DoesExist(x => x.Name.English == request.EnglishName && x.Id != request.ProductId);
            if (doesEnglishNameExist)
                return new Result<Product>()
                    .WithBadRequest(SharedResourcesKeys.DoesExist.Localize(SharedResourcesKeys.NameEn.Localize()));

            var englishNameResult = productToBeEdited.Name.UpdateEnglish(request.EnglishName);
            if (englishNameResult.IsFailed)
                return englishNameResult;
        }

        if (!string.IsNullOrEmpty(request.ArabicName))
        {
            var doesArabicNameExist = await _productRepository.DoesExist(x => x.Name.Arabic == request.ArabicName && x.Id != request.ProductId);
            if (doesArabicNameExist)
                return new Result<Product>()
                    .WithBadRequest(SharedResourcesKeys.DoesExist.Localize(SharedResourcesKeys.NameAr.Localize()));

            var arabicNameResult = productToBeEdited.Name.UpdateArabic(request.ArabicName);
            if (arabicNameResult.IsFailed)
                return arabicNameResult;
        }

        if (!string.IsNullOrWhiteSpace(request.ModelNumber))
        {
            var doesModelNumberExist = await _productRepository.DoesExist(x => x.ModelNumber == request.ModelNumber);
            if (!doesModelNumberExist)
                return new Result<Product>()
                    .WithBadRequest(SharedResourcesKeys.DoesExist.Localize(SharedResourcesKeys.ModelNumber.Localize()));

            productToBeEdited.UpdateModelNumber(request.ModelNumber);
        }

        if (!string.IsNullOrWhiteSpace(request.Description))
        {
            productToBeEdited.UpdateDescription(request.Description);
        }

        if (request.BrandId.HasValue && request.BrandId.Value > 0)
        {
            var doesBrandExist = await _brandRepository.DoesExist(x => x.Id == request.BrandId);
            if (!doesBrandExist)
                return new Result<Product>()
                    .WithBadRequest(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.Brand.Localize()));

            productToBeEdited.UpdateBrand(request.BrandId.Value);
        }

        if (request.CategoryId.HasValue && request.CategoryId.Value > 0)
        {
            var category = await _categoryRepository.GetByID(request.CategoryId.Value);
            if (category == null)
                return new Result<Product>()
                    .WithBadRequest(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.Category.Localize()));

            if (!category.IsLeaf)
                return new Result<Product>()
                    .WithBadRequest(SharedResourcesKeys.CategoryMustBeLeaf.Localize());

            productToBeEdited.UpdateCategory(request.CategoryId.Value);
        }

        if (request.ShelfLifeInDays.HasValue && request.ShelfLifeInDays.Value > 0)
        {
            productToBeEdited.UpdateShelfLife(request.ShelfLifeInDays.Value);
        }

        if (request.WarrantyInDays.HasValue && request.WarrantyInDays.Value > 0)
        {
            productToBeEdited.UpdateWarranty(request.WarrantyInDays.Value);
        }

        _productRepository.Update(productToBeEdited);

        var productToBeEditedResult = await new Result<Product>()
            .WithTask(() => _unitOfWork.SaveChangesAsync(cancellationToken), SharedResourcesKeys.DatabaseError);
        if (productToBeEditedResult.IsFailed)
            return productToBeEditedResult;

        return productToBeEditedResult.WithUpdated();
    }

    public async Task<IResultBase> Handle(DeleteProductCommandModel request, CancellationToken cancellationToken)
    {
        var productToBeDeleted = await _productRepository.GetByID(request.ProductId);
        if (productToBeDeleted == null)
            return new Result<Product>()
                .WithBadRequest(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.Product.Localize()));

        _productRepository.Remove(productToBeDeleted);

        var productToBeDeletedResult = await new Result<Product>(productToBeDeleted)
            .WithTask(() => _unitOfWork.SaveChangesAsync(cancellationToken), SharedResourcesKeys.DatabaseError);
        if (productToBeDeletedResult.IsFailed)
            return productToBeDeletedResult;

        return productToBeDeletedResult.WithDeleted();
    }
}