using smERP.Domain.Entities.Product;
using smERP.Domain.Events;
using smERP.SharedKernel.Localizations.Extensions;
using smERP.SharedKernel.Localizations.Resources;
using smERP.SharedKernel.Responses;
using System.Net;
using System.Transactions;

namespace smERP.Domain.Entities.InventoryTransaction;

public class InventoryTransactionItem : Entity
{
    public int TransactionId { get; private set; }
    public int ProductInstanceId { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    //public Discount? ItemDiscount { get; private set; }
    public decimal TotalPrice => Quantity * UnitPrice; //ItemDiscount?.Apply(Quantity * UnitPrice) ?? (Quantity * UnitPrice);
    public virtual ProductInstance ProductInstance { get; private set; } = null!;
    public ICollection<InventoryTransactionItemUnit>? InventoryTransactionItemUnits { get; private set; }

    private InventoryTransactionItem(int productInstanceId, int quantity, decimal unitPrice, ICollection<InventoryTransactionItemUnit>? inventoryTransactionItemUnits)
    {
        ProductInstanceId = productInstanceId;
        Quantity = quantity;
        UnitPrice = unitPrice;
        InventoryTransactionItemUnits = inventoryTransactionItemUnits;
    }

    internal InventoryTransactionItem() { }

    //public void ApplyDiscount(Discount discount)
    //{
    //    ItemDiscount = discount;
    //}

    public static IResult<InventoryTransactionItem> Create(int productInstanceId, int quantity, decimal unitPrice, bool isTracked, List<string>? serialNumbers)
    {
        if (quantity < 0)
            return new Result<InventoryTransactionItem>()
                .WithError(SharedResourcesKeys.Required_FieldName.Localize(SharedResourcesKeys.Quantity.Localize()))
                .WithStatusCode(HttpStatusCode.BadRequest);

        if (unitPrice < 0)
            return new Result<InventoryTransactionItem>()
                .WithError(SharedResourcesKeys.Required_FieldName.Localize(SharedResourcesKeys.Price.Localize()))
                .WithStatusCode(HttpStatusCode.BadRequest);

        if (!isTracked)
            return new Result<InventoryTransactionItem>(new InventoryTransactionItem(productInstanceId, quantity, unitPrice, null));

        if (serialNumbers == null || serialNumbers.Distinct().Count() != serialNumbers.Count)
            return new Result<InventoryTransactionItem>()
                .WithError(SharedResourcesKeys.___ListCannotContainDuplicates.Localize(SharedResourcesKeys.SerialNumber.Localize()))
                .WithStatusCode(HttpStatusCode.BadRequest);

        if (serialNumbers.Count != quantity)
            return new Result<InventoryTransactionItem>()
                .WithError(SharedResourcesKeys.SomeItemsIn___ListAreNotCorrect.Localize(SharedResourcesKeys.SerialNumber.Localize()))
                .WithStatusCode(HttpStatusCode.BadRequest);

        var inventoryTransactionItemUnits = serialNumbers.Select(serialNumber => new InventoryTransactionItemUnit(productInstanceId, serialNumber)).ToList();

        return new Result<InventoryTransactionItem>(new InventoryTransactionItem(productInstanceId, quantity, unitPrice, inventoryTransactionItemUnits));
    }

    public IResult<InventoryTransactionItem> Update(decimal? unitPrice, int? quantity, bool isTracked, List<string>? serialNumbers)
    {
        if (quantity != null && quantity.HasValue)
        {
            if (quantity < 0)
                return new Result<InventoryTransactionItem>()
                    .WithError(SharedResourcesKeys.Required_FieldName.Localize(SharedResourcesKeys.Quantity.Localize()))
                    .WithStatusCode(HttpStatusCode.BadRequest);

            var oldQuantity = Quantity;

            Quantity = quantity.Value;
        }

        if (unitPrice != null && unitPrice.HasValue)
        {
            if (unitPrice < 0)
                return new Result<InventoryTransactionItem>()
                    .WithError(SharedResourcesKeys.Required_FieldName.Localize(SharedResourcesKeys.Price.Localize()))
                    .WithStatusCode(HttpStatusCode.BadRequest);

            UnitPrice = unitPrice.Value;
        }

        if (!isTracked)
            return new Result<InventoryTransactionItem>(this);

        if (serialNumbers == null || serialNumbers.Distinct().Count() != serialNumbers.Count)
            return new Result<InventoryTransactionItem>()
                .WithError(SharedResourcesKeys.___ListCannotContainDuplicates.Localize(SharedResourcesKeys.SerialNumber.Localize()))
                .WithStatusCode(HttpStatusCode.BadRequest);

        if (serialNumbers.Count != quantity)
            return new Result<InventoryTransactionItem>()
                .WithError(SharedResourcesKeys.SomeItemsIn___ListAreNotCorrect.Localize(SharedResourcesKeys.SerialNumber.Localize()))
                .WithStatusCode(HttpStatusCode.BadRequest);

        InventoryTransactionItemUnits = serialNumbers.Select(serialNumber => new InventoryTransactionItemUnit(TransactionId, ProductInstanceId, serialNumber)).ToList();

        return new Result<InventoryTransactionItem>(this);
    }

    internal IResult<InventoryTransactionItemUnit> AddUnit(string serialNumber)
    {
        InventoryTransactionItemUnits ??= new List<InventoryTransactionItemUnit>();

        var doesUnitExist = InventoryTransactionItemUnits.Any(x => x.SerialNumber == serialNumber);
        if (doesUnitExist)
            return new Result<InventoryTransactionItemUnit>()
                .WithBadRequestResult(SharedResourcesKeys.DoesExist.Localize(SharedResourcesKeys.Product.Localize()));

        InventoryTransactionItemUnits.Add(new InventoryTransactionItemUnit(ProductInstanceId, serialNumber));

        return new Result<InventoryTransactionItemUnit>();
    }

    internal IResult<InventoryTransactionItemUnit> RemoveUnit(string serialNumber)
    {
        if (InventoryTransactionItemUnits == null)
            return new Result<InventoryTransactionItemUnit>()
                .WithBadRequestResult(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.Product.Localize()));

        var unitToBeRemoved = InventoryTransactionItemUnits.FirstOrDefault(x => x.SerialNumber == serialNumber);
        if (unitToBeRemoved == null)
            return new Result<InventoryTransactionItemUnit>()
                .WithBadRequestResult(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.Product.Localize()));

        InventoryTransactionItemUnits.Remove(unitToBeRemoved);

        return new Result<InventoryTransactionItemUnit>();
    }

    internal void UpdateQuantity(int quantity)
    {
        Quantity = quantity;
    }

    internal void UpdateUnitPrice(decimal unitPrice)
    {
        UnitPrice = unitPrice;
    }

    public void InventoryTransactionItemUnitsInitializing()
    {
        InventoryTransactionItemUnits = new List<InventoryTransactionItemUnit>();
    }
}
