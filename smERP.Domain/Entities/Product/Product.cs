using smERP.Domain.ValueObjects;
using smERP.SharedKernel.Localizations.Extensions;
using smERP.SharedKernel.Localizations.Resources;
using smERP.SharedKernel.Responses;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace smERP.Domain.Entities.Product;

public class Product : Entity, IAggregateRoot
{
    public BilingualName Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public string ModelNumber { get; private set; } = null!;
    public int? ShelfLifeInDays { get; private set; }
    public int? WarrantyInDays { get; private set; }
    public bool DoesExpire => ShelfLifeInDays.HasValue;
    public bool IsWarranted => WarrantyInDays.HasValue;
    public bool AreItemsTracked => DoesExpire || IsWarranted;
    public int BrandId { get; private set; }
    public int CategoryId { get; private set; }
    public virtual Brand Brand { get; private set; } = null!;
    public virtual Category Category { get; private set; } = null!;
    public virtual ICollection<ProductInstance> ProductInstances { get; private set; } = new List<ProductInstance>();
    public virtual ICollection<ProductSupplier> ProductSuppliers { get; private set; } = new List<ProductSupplier>();

    private Product(BilingualName name, string modelNumber, int brandId, int categoryId, string? description = null, int? shelfLifeInDays = null, int? warrantyInDays = null)
    {
        Name = name;
        ModelNumber = modelNumber;
        BrandId = brandId;
        CategoryId = categoryId;
        Description = description;
        ShelfLifeInDays = shelfLifeInDays;
        WarrantyInDays = warrantyInDays;
    }

    private Product() { }

    public static IResult<Product> Create(string englishName, string arabicName, string modelNumber, int brandId, int categoryId, string? description = null, int? shelfLifeInDays = null, int? warrantyInDays = null)
    {

        var nameResult = BilingualName.Create(englishName, arabicName);
        if (nameResult.IsFailed)
            return nameResult.ChangeType(new Product());

        if (string.IsNullOrWhiteSpace(modelNumber))
        {
            var localizedModelNumber = SharedResourcesKeys.ModelNumber.Localize();
            var result = new Result<Product>()
                .WithError(SharedResourcesKeys.Required_FieldName.Localize(localizedModelNumber))
                .WithMessage(SharedResourcesKeys.BadRequest.Localize())
                .WithStatusCode(HttpStatusCode.BadRequest);
            return result;
        }

        return new Result<Product>(new Product(nameResult.Value, modelNumber, brandId, categoryId, description, shelfLifeInDays, warrantyInDays))
            .WithStatusCode(HttpStatusCode.Created)
            .WithMessage(SharedResourcesKeys.Created.Localize());
    }

    public void UpdateDescription(string description)
    {
        Description = description;
    }

    public void UpdateModelNumber(string modelNumber)
    {
        ModelNumber = modelNumber;
    }

    public void UpdateCategory(int categoryId)
    {
        CategoryId = categoryId;
    }

    public void UpdateBrand(int brandId)
    {
        BrandId = brandId;
    }

    public void UpdateShelfLife(int shelfLife)
    {
        ShelfLifeInDays = shelfLife;
    }

    public void UpdateWarranty(int warranty)
    {
        WarrantyInDays = warranty;
    }

    //public IResult<Product> UpdateDetails(string englishName, string arabicName, string modelNumber, string? description)
    //{
    //    var nameResult = BilingualName.Create(englishName, arabicName);
    //    if (nameResult.IsFailed)
    //        return nameResult.ChangeType(new Product());

    //    if (string.IsNullOrWhiteSpace(modelNumber))
    //    {
    //        var localizedModelNumber = SharedResourcesKeys.ModelNumber.Localize();
    //        var result = new Result<Product>()
    //            .WithError(SharedResourcesKeys.Required_FieldName.Localize(localizedModelNumber))
    //            .WithMessage(SharedResourcesKeys.BadRequest.Localize())
    //            .WithStatusCode(HttpStatusCode.BadRequest);
    //        return result;
    //    }

    //    Name = nameResult.Value;
    //    ModelNumber = modelNumber;
    //    Description = description;

    //    return new Result<Product>(this)
    //        .WithStatusCode(HttpStatusCode.OK)
    //        .WithMessage(SharedResourcesKeys.UpdatedSuccess.Localize());
    //}

    public IResult<ProductInstance> AddProductInstance(decimal sellingPrice, List<(int attributeId, int attributeValueId)> attributeValuesIds)
    {
        if (!IsAttributeValuesIdsListUnique(attributeValuesIds))
            return new Result<ProductInstance>()
                .WithError(SharedResourcesKeys.___ListCannotContainDuplicates.Localize(SharedResourcesKeys.AttributeValue.Localize()))
                .WithStatusCode(HttpStatusCode.BadRequest);

        var newProductInstanceSku = GenerateSku(attributeValuesIds);
        if(IsProductInstanceNotUnique(newProductInstanceSku))
            return new Result<ProductInstance>()
                .WithError(SharedResourcesKeys.ProductInstanceDuplicate.Localize())
                .WithStatusCode(HttpStatusCode.BadRequest);

        var productInstanceResult = ProductInstance.Create(Id, newProductInstanceSku, 0, 0, sellingPrice, attributeValuesIds);
        if (productInstanceResult.IsFailed)
            return productInstanceResult;

        ProductInstances.Add(productInstanceResult.Value);

        return productInstanceResult;
    }

    public IResult<ProductInstance> UpdateProductInstanceAttribute(int productInstanceId, List<(int attributeId, int attributeValueId)> attributeValuesIds)
    {
        var productInstanceToBeEdited = ProductInstances.FirstOrDefault(x => x.Id == productInstanceId);
        if (productInstanceToBeEdited == null)
            return new Result<ProductInstance>()
                .WithError(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.Product.Localize()))
                .WithStatusCode(HttpStatusCode.BadRequest);

        if (!IsAttributeValuesIdsListUnique(attributeValuesIds))
            return new Result<ProductInstance>()
                .WithError(SharedResourcesKeys.___ListCannotContainDuplicates.Localize(SharedResourcesKeys.AttributeValue.Localize()))
                .WithStatusCode(HttpStatusCode.BadRequest);

        var newProductInstanceSku = GenerateSku(attributeValuesIds);
        if (IsProductInstanceNotUnique(newProductInstanceSku, productInstanceId))
            return new Result<ProductInstance>()
                .WithError(SharedResourcesKeys.ProductInstanceDuplicate.Localize())
                .WithStatusCode(HttpStatusCode.BadRequest);

        var productInstanceEditResult = productInstanceToBeEdited.UpdateAttributeValues(attributeValuesIds);
        return productInstanceEditResult;
    }

    public IResult<ProductInstance> RemoveProductInstance(int productInstanceId)
    {
        var productInstanceToBeDeleted = ProductInstances.FirstOrDefault(x => x.Id == productInstanceId);
        if (productInstanceToBeDeleted == null)
            return new Result<ProductInstance>()
                .WithError(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.Product.Localize()))
                .WithStatusCode(HttpStatusCode.BadRequest);

        ProductInstances.Remove(productInstanceToBeDeleted);
        return new Result<ProductInstance>()
            .WithError(SharedResourcesKeys.DeletedSuccess.Localize())
            .WithStatusCode(HttpStatusCode.NoContent);
    }

    private string GenerateSku(List<(int attributeId, int attributeValueId)> attributeValuesIds)
    {
        var sku = new StringBuilder(Id.ToString() + "-");

        var orderedAttributes = attributeValuesIds
            .OrderBy(a => a.attributeId);

        foreach (var attr in orderedAttributes)
        {
            sku.Append($"{attr.attributeId},{attr.attributeValueId}-");
        }

        if (sku.Length > 0 && sku[sku.Length - 1] == '-')
        {
            sku.Length--;
        }

        return sku.ToString();
    }

    private bool IsProductInstanceNotUnique(string newProductInstanceSku) 
    {
        if (ProductInstances.Count == 0)
            return false;

        var existingProductInstanceSkus = ProductInstances.Select(x => x.Sku);
        return existingProductInstanceSkus.Any(x => x == newProductInstanceSku);
    }

    private bool IsProductInstanceNotUnique(string newProductInstanceSku, int productInstanceId)
    {
        var existingProductInstanceSkus = ProductInstances.Select(x => new { x.Sku, x.Id });
        return existingProductInstanceSkus.Any(x => x.Sku == newProductInstanceSku && x.Id != productInstanceId);
    }

    private bool IsAttributeValuesIdsListUnique(List<(int attributeId, int attributeValueId)> attributeValuesIds)
    {
        return attributeValuesIds.DistinctBy(x => x.attributeId).Count() == attributeValuesIds.Count;
    }

    //public (int AttributeId, int AttributeValueId)? GetFirstUniqueCombination(string targetSku, List<string> skuList)
    //{
    //    var targetAttributes = ParseAttributes(targetSku);
    //    var skuAttributes = ProductInstances.Select(x => x.Sku).Select(ParseAttributes).ToList();

    //    foreach (var (key, value) in targetAttributes)
    //    {
    //        if (skuAttributes.All(attrs => !attrs.TryGetValue(key, out var v) || v != value))
    //        {
    //            return (key , value);
    //        }
    //    }

    //    return null;
    //}

    //private static Dictionary<int, int> ParseAttributes(string sku)
    //{
    //    return sku.Split('-')
    //        .Skip(1)
    //        .Select(part => part.Split(':'))
    //        .Where(split => split.Length == 2)
    //        .ToDictionary(
    //            split => int.Parse(split[0]),
    //            split => int.Parse(split[1])
    //        );
    //}
}