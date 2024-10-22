using MediatR;
using smERP.Application.Contracts.Persistence;
using smERP.Application.Features.ProductInstances.Commands.Models;
using smERP.Domain.Entities.Product;
using smERP.Domain.ValueObjects;
using smERP.SharedKernel.Enums;
using smERP.SharedKernel.Localizations.Extensions;
using smERP.SharedKernel.Localizations.Resources;
using smERP.SharedKernel.Responses;
using System.Collections.Generic;

namespace smERP.Application.Features.ProductInstances.Commands.Handlers;

public class ProductInstanceCommandHandler(IProductRepository productRepository, IAttributeRepository attributeRepository, IFileStorageRepository fileStorageRepository, IUnitOfWork unitOfWork) :
    IRequestHandler<AddProductInstanceCommandModel, IResultBase>,
    IRequestHandler<EditProductInstanceCommandModel, IResultBase>,
    IRequestHandler<DeleteProductInstanceCommandModel, IResultBase>
{
    private readonly IProductRepository _productRepository = productRepository;
    private readonly IAttributeRepository _attributeRepository = attributeRepository;
    private readonly IFileStorageRepository _fileStorageRepository = fileStorageRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<IResultBase> Handle(AddProductInstanceCommandModel request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdWithProductInstances(request.ProductId);
        if (product == null)
            return new Result<ProductInstance>()
                .WithBadRequest(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.Product.Localize()));

        var attributeValuesList = request.Attributes.Select(av => (av.AttributeId, av.AttributeValueId)).ToList();
        var doesAttributeValuesExist = await _attributeRepository.DoesListExist(attributeValuesList);
        if (!doesAttributeValuesExist)
            return new Result<ProductInstance>()
                .WithBadRequest(SharedResourcesKeys.SomeItemsIn___ListAreNotCorrect.Localize(SharedResourcesKeys.AttributeList.Localize()));

        var productInstanceToBeCreatedResult = product.AddProductInstance(request.SellingPrice ?? 0, attributeValuesList);
        if (productInstanceToBeCreatedResult.IsFailed)
            return productInstanceToBeCreatedResult;

        if (request.ImagesBase64 != null && request.ImagesBase64.Count > 0)
        {
            var imageMemoryStreams = Image.CreateMemoryStreams(request.ImagesBase64);
            if (imageMemoryStreams.IsFailed)
                return imageMemoryStreams;

            var images = new List<Image>();

            foreach (var image in imageMemoryStreams.Value)
            {
                var savedImagePath = await _fileStorageRepository.StoreFile(image.MemoryStream, FileType.ProductImage, $"{product.Name.English}-{productInstanceToBeCreatedResult.Value.Sku}.{image.ContentType}", cancellationToken);
                if (savedImagePath == null)
                    return new Result<ProductInstance>()
                        .WithBadRequest(SharedResourcesKeys.ThereSomeThingWrongWithImageProvided.Localize());

                image.MemoryStream.Dispose();

                images.Add(Image.Create(savedImagePath));
            }

            productInstanceToBeCreatedResult.Value.AddImages(images);
        }

        _productRepository.Update(product);

        await productInstanceToBeCreatedResult.WithTask(() => _unitOfWork.SaveChangesAsync(cancellationToken), SharedResourcesKeys.DatabaseError);
        if (productInstanceToBeCreatedResult.IsFailed)
        {
            foreach (var image in productInstanceToBeCreatedResult.Value.Images)
            {
                await _fileStorageRepository.DeleteFile(image.Path, CancellationToken.None);
            }

            return productInstanceToBeCreatedResult;
        }

        return productInstanceToBeCreatedResult.ChangeType(productInstanceToBeCreatedResult.Value.Id).WithCreated();
    }

    public async Task<IResultBase> Handle(EditProductInstanceCommandModel request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdWithProductInstances(request.ProductId);
        if (product == null)
            return new Result<ProductInstance>()
                .WithBadRequest(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.Product.Localize()));

        var productInstanceToBeEdited = product.ProductInstances.FirstOrDefault(x => x.Id == request.ProductInstanceId);
        if (productInstanceToBeEdited == null)
            return new Result<ProductInstance>()
                .WithBadRequest(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.Product.Localize()));

        IResult<ProductInstance> productInstanceToBeEditedResult = new Result<ProductInstance>(productInstanceToBeEdited);

        var doesRequestAddImages = request.ImagesBase64 != null && request.ImagesBase64.Count > 0;

        if (request.ImagesPathToRemove != null && request.ImagesPathToRemove.Count > 0)
        {
            if ((request.ImagesPathToRemove.Count == productInstanceToBeEdited.Images.Count) && !doesRequestAddImages)
                return productInstanceToBeEditedResult.WithBadRequest(SharedResourcesKeys.ProductInstanceMustHaveAtleastOneImage.Localize());

            foreach(var imagePathToRemove in request.ImagesPathToRemove)
            {
                var imageToRemove = productInstanceToBeEdited.Images.FirstOrDefault(image => image.Path.Equals(imagePathToRemove, StringComparison.CurrentCultureIgnoreCase));
                if (imageToRemove is null)
                    return productInstanceToBeEditedResult.WithBadRequest(SharedResourcesKeys.Invalid___.Localize(SharedResourcesKeys.Image.Localize()));

                var isImageDeleted = await _fileStorageRepository.DeleteFile(imagePathToRemove, cancellationToken);
                if (!isImageDeleted)
                    return productInstanceToBeEditedResult.WithBadRequest(SharedResourcesKeys.FileWasNotDeletedPleaseTryAgainLater.Localize());

                var RemovingImageFromInstanceResult = productInstanceToBeEdited.RemoveImages([imageToRemove.Path]);
                if (RemovingImageFromInstanceResult.IsFailed)
                    return RemovingImageFromInstanceResult;
            }
        }

        if (doesRequestAddImages)
        {
            var imageMemoryStreams = Image.CreateMemoryStreams(request.ImagesBase64 ?? []);
            if (imageMemoryStreams.IsFailed)
                return imageMemoryStreams;

            var images = new List<Image>();

            foreach (var image in imageMemoryStreams.Value)
            {
                var savedImagePath = await _fileStorageRepository.StoreFile(image.MemoryStream, FileType.ProductImage, $"{product.Name.English}-{productInstanceToBeEditedResult.Value.Sku}.{image.ContentType}", cancellationToken);
                if (savedImagePath == null)
                    return new Result<ProductInstance>()
                        .WithBadRequest(SharedResourcesKeys.ThereSomeThingWrongWithImageProvided.Localize());

                image.MemoryStream.Dispose();

                images.Add(Image.Create(savedImagePath));
            }

            productInstanceToBeEditedResult.Value.AddImages(images);
        }
        //if (request.QuantityInStock != null && request.QuantityInStock.HasValue && request.QuantityInStock > -1)
        //{
        //    productInstanceToBeEditedResult = productInstanceToBeEdited.UpdateQuantity(request.QuantityInStock.Value);
        //    if (productInstanceToBeEditedResult.IsFailed)
        //        return productInstanceToBeEditedResult;
        //}

        //if (request.BuyingPrice != null && request.BuyingPrice.HasValue && request.BuyingPrice > -1)
        //{
        //    productInstanceToBeEditedResult = productInstanceToBeEdited.UpdateBuyingPrice(request.BuyingPrice.Value);
        //    if (productInstanceToBeEditedResult.IsFailed)
        //        return productInstanceToBeEditedResult;
        //}

        if (request.SellingPrice != null && request.SellingPrice.HasValue && request.SellingPrice > -1)
        {
            productInstanceToBeEditedResult = productInstanceToBeEdited.UpdateSellingPrice(request.SellingPrice.Value);
            if (productInstanceToBeEditedResult.IsFailed)
                return productInstanceToBeEditedResult;
        }

        if (request.Attributes != null && request.Attributes.Count > 0)
        {
            var attributeValuesList = request.Attributes.Select(av => (av.AttributeId, av.AttributeValueId)).ToList();
            productInstanceToBeEditedResult = product.UpdateProductInstanceAttribute(request.ProductInstanceId, attributeValuesList);
            if (productInstanceToBeEditedResult.IsFailed)
                return productInstanceToBeEditedResult;
        }

        _productRepository.Update(product);

        await productInstanceToBeEditedResult.WithTask(() => _unitOfWork.SaveChangesAsync(cancellationToken), SharedResourcesKeys.DatabaseError);
        if (productInstanceToBeEditedResult.IsFailed)
            return productInstanceToBeEditedResult;

        return productInstanceToBeEditedResult.WithUpdated();
    }

    public async Task<IResultBase> Handle(DeleteProductInstanceCommandModel request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdWithProductInstances(request.ProductId);
        if (product == null)
            return new Result<ProductInstance>()
                .WithBadRequest(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.Product.Localize()));

        var productToBeDeletedResult = product.RemoveProductInstance(request.ProductInstanceId);
        if (productToBeDeletedResult.IsSuccess)
            return productToBeDeletedResult;

        _productRepository.Update(product);

        await productToBeDeletedResult.WithTask(() => _unitOfWork.SaveChangesAsync(cancellationToken), SharedResourcesKeys.DatabaseError);
        if (productToBeDeletedResult.IsFailed)
            return productToBeDeletedResult;

        return productToBeDeletedResult.WithDeleted();
    }
}
