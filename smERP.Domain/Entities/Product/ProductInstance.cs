
using smERP.Domain.ValueObjects;
using smERP.SharedKernel.Localizations.Extensions;
using smERP.SharedKernel.Localizations.Resources;
using smERP.SharedKernel.Responses;
using System.Net;
using System.Text;

namespace smERP.Domain.Entities.Product;

public class ProductInstance : Entity, IAggregateRoot
{
    public int ProductId { get; private set; }
    public string? Sku { get; private set; }
    public int QuantityInStock { get; private set; }
    public decimal BuyingPrice { get; private set; }
    public decimal SellingPrice { get; private set; }
    public int? DefaultQuantityAlertLevel { get; private set; }
    //public int FirstUniqueAttributeId { get; private set; }
    //public int FirstUniqueAttributeValueId { get; private set; }
    public virtual Product Product { get; private set; } = null!;
    public virtual ICollection<Image> Images { get; private set; } = new List<Image>();
    public virtual ICollection<ProductInstanceAttributeValue> ProductInstanceAttributeValues { get; private set; } = new List<ProductInstanceAttributeValue>();


    //private ProductInstance(int productId, int quantityInStock, decimal buyingPrice, decimal sellingPrice, List<ProductInstanceAttributeValue> productInstanceAttributeValues)
    //{
    //    ProductId = productId;
    //    QuantityInStock = quantityInStock;
    //    BuyingPrice = buyingPrice;
    //    SellingPrice = sellingPrice;
    //    ProductInstanceAttributeValues = productInstanceAttributeValues;
    //}

    private ProductInstance(int productId, string sku, int quantityInStock, decimal buyingPrice, decimal sellingPrice, List<ProductInstanceAttributeValue> productInstanceAttributeValues)
    {
        ProductId = productId;
        Sku = sku;
        QuantityInStock = quantityInStock;
        BuyingPrice = buyingPrice;
        SellingPrice = sellingPrice;
        //FirstUniqueAttributeId = firstUniqueAttributeId;
        //FirstUniqueAttributeValueId = firstUniqueAttributeValueId;
        ProductInstanceAttributeValues = productInstanceAttributeValues;
    }

    private ProductInstance() { }

    internal static IResult<ProductInstance> Create(int productId, string sku, int quantityInStock, decimal buyingPrice, decimal sellingPrice, List<(int AttributeId, int AttributeValueId)> attributeValuesIds)
    {
        if (quantityInStock < 0)
            return new Result<ProductInstance>()
                .WithError(SharedResourcesKeys.___MustBeAPositiveNumber.Localize(SharedResourcesKeys.Quantity.Localize()))
                .WithStatusCode(HttpStatusCode.BadRequest);

        if (buyingPrice < 0)
            return new Result<ProductInstance>()
                .WithError(SharedResourcesKeys.___MustBeAPositiveNumber.Localize(SharedResourcesKeys.BuyingPrice.Localize()))
                .WithStatusCode(HttpStatusCode.BadRequest);

        if (sellingPrice < 0)
            return new Result<ProductInstance>()
                .WithError(SharedResourcesKeys.___MustBeAPositiveNumber.Localize(SharedResourcesKeys.SellingPrice.Localize()))
                .WithStatusCode(HttpStatusCode.BadRequest);

        var attributeValues = attributeValuesIds.Select(x => new ProductInstanceAttributeValue(x.AttributeId, x.AttributeValueId)).ToList();

        var productInstance = new ProductInstance(productId, sku, quantityInStock, buyingPrice, sellingPrice, attributeValues);
        return new Result<ProductInstance>(productInstance)
            .WithStatusCode(HttpStatusCode.Created)
            .WithMessage(SharedResourcesKeys.Created.Localize());
    }

    //public IResult<ProductInstance> UpdateQuantity(int newQuantity)
    //{
    //    if (newQuantity < 0)
    //        return new Result<ProductInstance>().WithError(SharedResourcesKeys.___MustBeAPositiveNumber.Localize(SharedResourcesKeys.Quantity.Localize()))
    //            .WithStatusCode(HttpStatusCode.BadRequest);

    //    QuantityInStock = newQuantity;
    //    return new Result<ProductInstance>(this)
    //        .WithStatusCode(HttpStatusCode.NoContent)
    //        .WithMessage(SharedResourcesKeys.UpdatedSuccess.Localize());
    //}

    //public IResult<ProductInstance> UpdateBuyingPrice(decimal buyingPrice)
    //{
    //    if (buyingPrice < 0)
    //        return new Result<ProductInstance>()
    //            .WithError(SharedResourcesKeys.___MustBeAPositiveNumber.Localize(SharedResourcesKeys.BuyingPrice.Localize()))
    //            .WithStatusCode(HttpStatusCode.BadRequest);

    //    BuyingPrice = buyingPrice;
    //    return new Result<ProductInstance>(this)
    //        .WithStatusCode(HttpStatusCode.NoContent)
    //        .WithMessage(SharedResourcesKeys.UpdatedSuccess.Localize());
    //}

    public IResult<ProductInstance> UpdateSellingPrice(decimal sellingPrice)
    {
        if (sellingPrice < 0)
            return new Result<ProductInstance>()
                .WithError(SharedResourcesKeys.___MustBeAPositiveNumber.Localize(SharedResourcesKeys.SellingPrice.Localize()))
                .WithStatusCode(HttpStatusCode.BadRequest);

        SellingPrice = sellingPrice;
        return new Result<ProductInstance>(this)
            .WithStatusCode(HttpStatusCode.NoContent)
            .WithMessage(SharedResourcesKeys.UpdatedSuccess.Localize());
    }

    public IResult<ProductInstance> UpdateAttributeValues(List<(int AttributeId, int AttributeValueId)> attributeValuesIds)
    {
        var attributeValues = attributeValuesIds.Select(x => new ProductInstanceAttributeValue(x.AttributeId, x.AttributeValueId)).ToList();
        ProductInstanceAttributeValues = attributeValues;
        return new Result<ProductInstance>(this)
            .WithStatusCode(HttpStatusCode.NoContent)
            .WithMessage(SharedResourcesKeys.UpdatedSuccess.Localize());
    }


    public void AddImages(List<Image> images)
    {
        foreach (var image in images)
        {
            Images.Add(image);
        }
    }

    public IResultBase UpdateImages(List<Image> images)
    {
        foreach (var image in images)
        {
            var imageToUpdate = Images.FirstOrDefault(x => x.Path == image.Path);
            if (imageToUpdate is null)
                return new Result<List<Image>>()
                    .WithBadRequestResult(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.Image.Localize()));

            imageToUpdate = image;
        }

        return new Result<List<Image>>();
    }

    public IResultBase RemoveImages(List<string> imagePaths)
    {
        foreach (var path in imagePaths)
        {
            var imageToRemove = Images.FirstOrDefault(x => x.Path == path);
            if (imageToRemove is null)
                return new Result<List<Image>>()
                    .WithBadRequestResult(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.Image.Localize()));

            Images.Remove(imageToRemove);
        }

        return new Result<List<Image>>();
    }

    public IResultBase SetAlertLevel(int AlertLevel)
    {
        if (AlertLevel < 1)
            return new Result<int>().WithBadRequest(SharedResourcesKeys.___MustBeAPositiveNumber.Localize(SharedResourcesKeys.Quantity.Localize()));

        DefaultQuantityAlertLevel = AlertLevel;
        return new Result<int>();
    }

    //public IResult<ProductInstanceAttribute> AddAttribute(int attributeValueId)
    //{
    //    var attributeCreateResult = ProductInstanceAttribute.Create(Id, attributeValueId);
    //    if (attributeCreateResult.IsFailed)
    //        return attributeCreateResult.ChangeType(new ProductInstanceAttribute());

    //    ProductInstanceAttributes.Add(attributeCreateResult.Value);
    //    return new Result<ProductInstanceAttribute>(attributeCreateResult.Value)
    //        .WithStatusCode(HttpStatusCode.Created)
    //        .WithMessage(SharedResourcesKeys.Created.Localize());
    //}

    //public void RemoveAttribute(ProductInstanceAttribute attribute)
    //{
    //    ProductInstanceAttributes.Remove(attribute);
    //}
}