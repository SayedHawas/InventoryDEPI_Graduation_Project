using smERP.Domain.Entities.Organization;
using smERP.SharedKernel.Localizations.Extensions;
using smERP.SharedKernel.Localizations.Resources;
using smERP.SharedKernel.Responses;
using System.Linq;
using System.Net;

namespace smERP.Domain.Entities.Product;

public class StoredProductInstance
{
    public int StorageLocationId { get; private set; }
    public int ProductInstanceId { get; private set; }
    public int Quantity { get; private set; }
    public bool IsTrackedByItem => Items != null && Items.Count > 0;
    public virtual ProductInstance ProductInstance { get; private set; } = null!;
    public ICollection<ProductInstanceItem>? Items { get; private set; }

    private StoredProductInstance(int storageLocationId, int productInstanceId, int quantity, ICollection<ProductInstanceItem>? items)
    {
        StorageLocationId = storageLocationId;
        ProductInstanceId = productInstanceId;
        Quantity = quantity;
        Items = items;
    }

    private StoredProductInstance() { }

    public static IResult<StoredProductInstance> Create(int storageLocationId, (int ProductInstanceId, int Quantity, bool IsTracked, int? ShelfLifeInDays, List<(string SerialNumber, string Status, DateOnly? ExpirationDate)>? Items) product)
    {
        if (product.Quantity < 0)
            return new Result<StoredProductInstance>()
                .WithError(SharedResourcesKeys.___MustBeAPositiveNumber.Localize(SharedResourcesKeys.Quantity.Localize()))
                .WithStatusCode(HttpStatusCode.BadRequest);

        if (!product.IsTracked)
            return CreateNonTrackedStoredProductInstance(storageLocationId, product.ProductInstanceId, product.Quantity);

        if (product.Items == null || product.Items.Count < 0)
            return new Result<StoredProductInstance>()
                .WithError(SharedResourcesKeys.___ListMustContainAtleastOneItem.Localize(SharedResourcesKeys.Product.Localize()))
                .WithStatusCode(HttpStatusCode.BadRequest);

        if (product.Items.DistinctBy(x => x.SerialNumber).Count() != product.Items.Count)
            return new Result<StoredProductInstance>()
                .WithError(SharedResourcesKeys.___ListCannotContainDuplicates.Localize(SharedResourcesKeys.SerialNumber.Localize()))
                .WithStatusCode(HttpStatusCode.BadRequest);

        if (product.Items.Count != product.Quantity)
            return new Result<StoredProductInstance>()
                .WithError(SharedResourcesKeys.SomeItemsIn___ListAreNotCorrect.Localize(SharedResourcesKeys.Product.Localize()))
                .WithStatusCode(HttpStatusCode.BadRequest);

        if (product.ShelfLifeInDays != null)
            return CreateTrackedStoredProductInstance(storageLocationId, product.ProductInstanceId, product.Quantity, product.ShelfLifeInDays.Value, product.Items);

        return CreateTrackedStoredProductInstance(storageLocationId, product.ProductInstanceId, product.Quantity, product.Items.Select(x => x.SerialNumber).ToList());
    }

    public IResult<StoredProductInstance> Update(int quantity, int? shelfLifeInDays, List<(string SerialNumber, string Status, DateOnly? ExpirationDate)>? items)
    {

        Quantity += quantity;

        if (!IsTrackedByItem)
        {
            return new Result<StoredProductInstance>(this);
        }

        if (items == null || items.Count() < 0)
            return new Result<StoredProductInstance>()
                .WithError(SharedResourcesKeys.___ListMustContainAtleastOneItem.Localize(SharedResourcesKeys.Product.Localize()))
                .WithStatusCode(HttpStatusCode.BadRequest);

        if (items.DistinctBy(x => x.SerialNumber).Count() != items.Count)
            return new Result<StoredProductInstance>()
                .WithError(SharedResourcesKeys.___ListCannotContainDuplicates.Localize(SharedResourcesKeys.SerialNumber.Localize()))
                .WithStatusCode(HttpStatusCode.BadRequest);

        var productInstanceItemsToBeCreated = new List<ProductInstanceItem>();

        if (shelfLifeInDays != null)
        {
            var maxExpirationDate = DateOnly.FromDateTime(DateTime.Now).AddDays(shelfLifeInDays.Value);

            if (items.Any(x => x.ExpirationDate > maxExpirationDate))
                return new Result<StoredProductInstance>()
                    .WithError(SharedResourcesKeys.EnteredExpirationDateCannotExceedProductShelfLife.Localize())
                    .WithStatusCode(HttpStatusCode.BadRequest);

            productInstanceItemsToBeCreated = items.Select(item => new ProductInstanceItem(StorageLocationId, ProductInstanceId, item.SerialNumber, item.Status, item.ExpirationDate ?? maxExpirationDate)).ToList();
            Items ??= [];
            foreach (var item in productInstanceItemsToBeCreated)
            {
                var existingItem = Items.FirstOrDefault(x => x.SerialNumber == item.SerialNumber);
                if (existingItem != null)
                {
                    existingItem.UpdateStatus(item.Status);
                }
                else
                {
                    Items.Add(item);
                }
            }

            return new Result<StoredProductInstance>(this);
        }

        productInstanceItemsToBeCreated = items.Select(item => new ProductInstanceItem(StorageLocationId, ProductInstanceId, item.SerialNumber, item.Status)).ToList();
        Items ??= [];

        foreach (var item in productInstanceItemsToBeCreated)
        {
            var existingItem = Items.FirstOrDefault(x => x.SerialNumber == item.SerialNumber);
            if (existingItem != null)
            {
                existingItem.UpdateStatus(item.Status);
            }
            else
            {
                Items.Add(item);
            }
        }

        return new Result<StoredProductInstance>(this);
    }

    private static IResult<StoredProductInstance> CreateTrackedStoredProductInstance(int storageLocationId, int productInstanceId, int quantity, int shelfLifeInDays, List<(string SerialNumber, string Status, DateOnly? ExpirationDate)> items)
    {
        var maxExpirationDate = DateOnly.FromDateTime(DateTime.Now).AddDays(shelfLifeInDays);

        if (items.Any(x => x.ExpirationDate > maxExpirationDate))
            return new Result<StoredProductInstance>()
                .WithError(SharedResourcesKeys.EnteredExpirationDateCannotExceedProductShelfLife.Localize())
                .WithStatusCode(HttpStatusCode.BadRequest);

        var productInstanceItemsToBeCreated = items.Select(item => new ProductInstanceItem(storageLocationId, productInstanceId, item.SerialNumber, item.Status, item.ExpirationDate ?? maxExpirationDate)).ToList();

        return new Result<StoredProductInstance>(new StoredProductInstance(storageLocationId, productInstanceId, quantity, productInstanceItemsToBeCreated));
    }

    private static IResult<StoredProductInstance> CreateTrackedStoredProductInstance(int storageLocationId, int productInstanceId, int quantity, List<string> itemSerialNumbers)
    {
        var productInstanceItemsToBeCreated = itemSerialNumbers.Select(item => new ProductInstanceItem(storageLocationId, productInstanceId, item, "Available")).ToList();

        return new Result<StoredProductInstance>(new StoredProductInstance(storageLocationId, productInstanceId, quantity, productInstanceItemsToBeCreated));
    }

    private static IResult<StoredProductInstance> CreateNonTrackedStoredProductInstance(int storageLocationId, int productInstanceId, int quantity)
    {
        return new Result<StoredProductInstance>(new StoredProductInstance(storageLocationId, productInstanceId, quantity, null));
    }
}