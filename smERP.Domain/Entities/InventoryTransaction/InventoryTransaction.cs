using smERP.Domain.Entities.Organization;
using smERP.Domain.Events;
using smERP.Domain.ValueObjects;
using smERP.SharedKernel.Localizations.Extensions;
using smERP.SharedKernel.Localizations.Resources;
using smERP.SharedKernel.Responses;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;

namespace smERP.Domain.Entities.InventoryTransaction;

[NotMapped]
public abstract class InventoryTransaction : Entity
{
    public int StorageLocationId { get; protected set; }
    public DateTime TransactionDate { get; protected set; }
    public decimal TotalAmount { get; protected set; }
    public bool IsCanceled { get; protected set; } = false;
    public bool IsTransactionProcessed { get; protected set; } = false;
    public virtual StorageLocation StorageLocation { get; protected set; } = null!;
    //public Discount? TransactionDiscount { get; protected set; }
    public ICollection<InventoryTransactionItem> Items { get; protected set; } = new List<InventoryTransactionItem>();

    protected InventoryTransaction(int storageLocationId, DateTime transactionDate, ICollection<InventoryTransactionItem> items)
    {
        StorageLocationId = storageLocationId;
        TransactionDate = transactionDate;
        Items = items;
        RecalculateAmount();
    }

    protected InventoryTransaction() { }

    public abstract string GetTransactionType();

    //public void ApplyTransactionDiscount(Discount discount)
    //{
    //    TransactionDiscount = discount;
    //    RecalculateAmount();
    //}

    public void AddItem(InventoryTransactionItem item)
    {
        Items.Add(item);
        RecalculateAmount();
    }

    public void TransactionProcessed()
    {
        IsTransactionProcessed = true;
    }

    public void UnderTransactionProcessing()
    {
        IsTransactionProcessed = false;
    }

    protected virtual void RecalculateAmount()
    {
        decimal subtotal = Items.Sum(item => item.TotalPrice);
        TotalAmount = subtotal;
        //Amount = TransactionDiscount?.Apply(subtotal) ?? subtotal;
    }

    protected static IResult<List<InventoryTransactionItem>> CreateBaseDetails(List<(int ProductInstanceId, int Quantity, decimal UnitPrice, bool IsTracked, List<string>? SerialNumbers)> transactionItems)
    {
        var inventoryTransactionItemsList = new List<InventoryTransactionItem>();

        foreach (var (productInstanceId, quantity, unitPrice, isTracked, serialNumbers) in transactionItems)
        {
            var inventoryTransactionItemsResult = InventoryTransactionItem.Create(productInstanceId, quantity, unitPrice, isTracked, serialNumbers);
            if (inventoryTransactionItemsResult.IsFailed)
                return new Result<List<InventoryTransactionItem>>()
                    .WithErrors(inventoryTransactionItemsResult.Errors)
                    .WithStatusCode(HttpStatusCode.BadRequest);

            inventoryTransactionItemsList.Add(inventoryTransactionItemsResult.Value);
        }

        return new Result<List<InventoryTransactionItem>>(inventoryTransactionItemsList);
    }

    public void RaiseTransactionCreatedEvent(List<(int ProductIsntanceId, int EffectedQuantity, bool IsTracked, int? ShelfLifeInDays, List<(string SerialNumber, string Status, DateOnly? ExpirationDate)>? SerialNumbers)> productEntries)
    {
        RaiseEvent(new ProductsQuantityChangedEvent(Id, StorageLocationId, GetTransactionType(), productEntries));
    }

    public IResult<List<InventoryTransactionItem>> AddItems(List<(int ProductInstanceId, int Quantity, decimal UnitPrice, bool IsTracked, int? ShelfLifeInDays, List<(string SerialNumber, DateOnly? ExpirationDate)>? Units)> items)
    {
        if (items.DistinctBy(x => x.ProductInstanceId).Count() == items.Count)
            return new Result<List<InventoryTransactionItem>>()
                .WithBadRequestResult(SharedResourcesKeys.___ListCannotContainDuplicates.Localize(SharedResourcesKeys.Product.Localize()));

        if (items.Any(x => x.Quantity < 0))
            return new Result<List<InventoryTransactionItem>>()
                .WithBadRequestResult(SharedResourcesKeys.___MustBeAPositiveNumber.Localize(SharedResourcesKeys.Quantity));

        if (items.Any(x => x.UnitPrice < 0))
            return new Result<List<InventoryTransactionItem>>()
                .WithBadRequestResult(SharedResourcesKeys.___MustBeAPositiveNumber.Localize(SharedResourcesKeys.Price));

        foreach (var item in items)
        {
            var doesItemExist = Items.Any(x => x.ProductInstanceId == item.ProductInstanceId);
            if (doesItemExist)
                return new Result<List<InventoryTransactionItem>>()
                    .WithBadRequestResult(SharedResourcesKeys.DoesExist.Localize(SharedResourcesKeys.Product.Localize()));

            var newItemResult = InventoryTransactionItem.Create(item.ProductInstanceId, item.Quantity, item.UnitPrice, item.IsTracked, item.Units?.Select(unit => unit.SerialNumber).ToList());
            if (newItemResult.IsFailed)
                return new Result<List<InventoryTransactionItem>>()
                    .WithErrors(newItemResult.Errors)
                    .WithStatusCode(HttpStatusCode.BadRequest);

            Items.Add(newItemResult.Value);
        }
        var newItems = items.Select(item => (
            item.ProductInstanceId,
            item.Quantity,
            item.IsTracked,
            item.ShelfLifeInDays,
            item.Units?.Select(unit => (unit.SerialNumber, "Available", unit.ExpirationDate)).ToList()
        )).ToList();

        RaiseEvent(new ProductsQuantityChangedEvent(Id, StorageLocationId, GetTransactionType(), newItems));

        return new Result<List<InventoryTransactionItem>>();
    }

    public IResult<List<InventoryTransactionItem>> UpdateItems(List<(int ProductInstanceId, int? Quantity, decimal? UnitPrice, bool IsTracked, int? ShelfLifeInDays, List<(string SerialNumber, DateOnly? ExpirationDate)>? UnitsToAdd, List<string>? UnitsToRemove)> items)
    {
        if (!(items.DistinctBy(x => x.ProductInstanceId).Count() == items.Count))
            return new Result<List<InventoryTransactionItem>>()
                .WithBadRequestResult(SharedResourcesKeys.___ListCannotContainDuplicates.Localize(SharedResourcesKeys.Product.Localize()));

        if (items.Any(x => x.UnitPrice < 0))
            return new Result<List<InventoryTransactionItem>>()
                .WithBadRequestResult(SharedResourcesKeys.___MustBeAPositiveNumber.Localize(SharedResourcesKeys.Price.Localize()));

        if (items.Any(x => x.IsTracked && ((x.UnitsToAdd?.Count ?? 0) - (x.UnitsToRemove?.Count ?? 0)) != x.Quantity))
            return new Result<List<InventoryTransactionItem>>()
                .WithBadRequestResult(SharedResourcesKeys.SomeItemsIn___ListAreNotCorrect.Localize(SharedResourcesKeys.Product.Localize()));

        foreach (var item in items)
        {
            var updateItemResult = UpdateItem(item);
            if (updateItemResult.IsFailed)
                return updateItemResult.ChangeType(new List<InventoryTransactionItem>());
        }

        DateOnly? nullDateOnly = null;

        var updatedItems = items.Select(item =>
        {
            var addedUnits = item.UnitsToAdd?.Select(unitToAdd => (unitToAdd.SerialNumber, "Available", unitToAdd.ExpirationDate)).ToList();

            var removedUnits = item.UnitsToRemove?.Select(unitToRemove => (unitToRemove, "Removed", nullDateOnly)).ToList();

            var combinedUnits = addedUnits?.Concat(removedUnits ?? []).ToList();

            return (
                item.ProductInstanceId,
                item.Quantity ?? 0,
                item.IsTracked,
                item.ShelfLifeInDays,
                combinedUnits
            );
        }).ToList();


        RaiseEvent(new ProductsQuantityChangedEvent(Id, StorageLocationId, GetTransactionType(), updatedItems));

        return new Result<List<InventoryTransactionItem>>();
    }

    public IResult<InventoryTransactionItem> RemoveItems(List<int> itemIds)
    {
        if (itemIds.Distinct().Count() != itemIds.Count)
            return new Result<InventoryTransactionItem>()
                .WithBadRequestResult(SharedResourcesKeys.___ListCannotContainDuplicates.Localize(SharedResourcesKeys.Product.Localize()));

        var itemsToBeRemoved = Items.Where(x => itemIds.Contains(x.ProductInstanceId)).ToList();

        if (itemsToBeRemoved.Count != itemIds.Count)
            return new Result<InventoryTransactionItem>()
                .WithBadRequestResult(SharedResourcesKeys.SomeItemsIn___ListAreNotCorrect.Localize(SharedResourcesKeys.Product.Localize()));

        foreach (var item in itemsToBeRemoved)
        {
            Items.Remove(item);
        }

        int? nullInt = null;
        DateOnly? nullDateOnly = null;

        var deletedItems = itemsToBeRemoved.Select(item => (item.ProductInstanceId, item.Quantity, item.InventoryTransactionItemUnits != null && item.InventoryTransactionItemUnits.Count > 0, nullInt, item.InventoryTransactionItemUnits?.Select(unit => (unit.SerialNumber, "Removed", nullDateOnly)).ToList()))
            .ToList();

        RaiseEvent(new ProductsQuantityChangedEvent(Id, StorageLocationId, GetTransactionType(), deletedItems));

        return new Result<InventoryTransactionItem>();
    }

    private IResult<InventoryTransactionItem> UpdateItem((int ProductInstanceId, int? Quantity, decimal? UnitPrice, bool IsTracked, int? ShelfLifeInDays, List<(string SerialNumber, DateOnly? ExpirationDate)>? UnitsToAdd, List<string>? UnitsToRemove) item)
    {
        var existingItem = Items.FirstOrDefault(x => x.ProductInstanceId == item.ProductInstanceId);
        if (existingItem == null)
            return new Result<InventoryTransactionItem>()
                .WithBadRequestResult(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.Product.Localize()));

        if (item.UnitsToRemove != null && item.UnitsToRemove.Count > 0 && (existingItem.InventoryTransactionItemUnits == null || existingItem.InventoryTransactionItemUnits.Count == 0))
            return new Result<InventoryTransactionItem>()
                .WithBadRequestResult(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.Product.Localize()));

        if (item.UnitsToRemove != null && item.UnitsToRemove.Count > 0)
        {
            foreach (var unit in item.UnitsToRemove)
            {
                var unitRemoveResult = existingItem.RemoveUnit(unit);
                if (unitRemoveResult.IsFailed)
                    return unitRemoveResult.ChangeType(new InventoryTransactionItem());
            }
        }

        if (item.UnitsToAdd != null && item.UnitsToAdd.Count > 0)
        {
            foreach (var unit in item.UnitsToAdd)
            {
                var unitAddResult = existingItem.AddUnit(unit.SerialNumber);
                if (unitAddResult.IsFailed)
                    return unitAddResult.ChangeType(new InventoryTransactionItem());
            }
        }

        if (item.Quantity != null)
            existingItem.UpdateQuantity(existingItem.Quantity + item.Quantity.Value);

        if (item.UnitPrice != null)
            existingItem.UpdateUnitPrice(item.UnitPrice.Value);

        return new Result<InventoryTransactionItem>();
    }

    public IResult<List<InventoryTransactionItem>> UpdateTransactionItems(List<(int ProductInstanceId, int Quantity, decimal UnitPrice, bool IsTracked, List<string>? SerialNumbers)> transactionItems)
    {
        //foreach (var (productInstanceId, quantity, unitPrice, isTracked, serialNumbers) in transactionItems)
        //{
        //    var existingInventoryTransactionItem = Items.FirstOrDefault(x => x.ProductInstanceId == productInstanceId);
        //    if (existingInventoryTransactionItem != null)
        //    {
        //        var oldQuantity = existingInventoryTransactionItem.Quantity;
        //        var effectedQuantity = quantity - oldQuantity;

        //        var updateResult = existingInventoryTransactionItem.Update(unitPrice, quantity, isTracked, serialNumbers);
        //        if (updateResult.IsFailed)
        //            return new Result<List<InventoryTransactionItem>>()
        //                .WithErrors(updateResult.Errors)
        //                .WithStatusCode(HttpStatusCode.BadRequest);

        //        continue;
        //    }

        //    RaiseEvent(new ProductsQuantityChangedEvent(Id, GetTransactionType(), (productInstanceId, effectedQuantity, isTracked, serialNumbers)));

        //    var newInventoryTransactionItem = InventoryTransactionItem.Create(productInstanceId, quantity, unitPrice, isTracked, serialNumbers);
        //    if (newInventoryTransactionItem.IsFailed)
        //        return new Result<List<InventoryTransactionItem>>()
        //            .WithErrors(newInventoryTransactionItem.Errors)
        //            .WithStatusCode(HttpStatusCode.BadRequest);

        //    Items.Add(newInventoryTransactionItem.Value);
        //}
        var updatedItems = new List<(int productInstanceId, int effectedQuantity, bool isTracked, List<string>? serialNumbers)>();

        foreach (var (productInstanceId, quantity, unitPrice, isTracked, serialNumbers) in transactionItems)
        {
            var existingInventoryTransactionItem = Items.FirstOrDefault(x => x.ProductInstanceId == productInstanceId);
            if (existingInventoryTransactionItem != null)
            {
                var oldQuantity = existingInventoryTransactionItem.Quantity;
                var effectedQuantity = quantity - oldQuantity;
                var updateResult = existingInventoryTransactionItem.Update(unitPrice, quantity, isTracked, serialNumbers);
                if (updateResult.IsFailed)
                    return new Result<List<InventoryTransactionItem>>()
                        .WithErrors(updateResult.Errors)
                        .WithStatusCode(HttpStatusCode.BadRequest);

                updatedItems.Add((productInstanceId, effectedQuantity, isTracked, serialNumbers));
            }
            else
            {
                var newInventoryTransactionItem = InventoryTransactionItem.Create(productInstanceId, quantity, unitPrice, isTracked, serialNumbers);
                if (newInventoryTransactionItem.IsFailed)
                    return new Result<List<InventoryTransactionItem>>()
                        .WithErrors(newInventoryTransactionItem.Errors)
                        .WithStatusCode(HttpStatusCode.BadRequest);

                Items.Add(newInventoryTransactionItem.Value);

                updatedItems.Add((productInstanceId, quantity, isTracked, serialNumbers));
            }
        }

        if (updatedItems.Count != 0)
        {
            //RaiseEvent(new ProductsQuantityChangedEvent(Id, GetTransactionType(), updatedItems));
        }

        RecalculateAmount();

        return new Result<List<InventoryTransactionItem>>(Items.ToList());
    }

    public IResultBase RemoveTransactionItems(List<int> productInstanceIds)
    {
        if (productInstanceIds.Distinct().Count() == productInstanceIds.Count)
            return new Result<List<InventoryTransactionItem>>()
                .WithError(SharedResourcesKeys.___ListCannotContainDuplicates.Localize(SharedResourcesKeys.Product.Localize()))
                .WithStatusCode(HttpStatusCode.BadRequest);

        var deletedItems = new List<(int productInstanceId, int effectedQuantity, bool isTracked, List<string>? serialNumbers)>();

        foreach (var productInstanceId in productInstanceIds)
        {
            var inventoryTransactionItemToBeRemoved = Items.FirstOrDefault(x => x.ProductInstanceId == productInstanceId);
            if (inventoryTransactionItemToBeRemoved == null)
                return new Result<List<InventoryTransactionItem>>()
                    .WithError(SharedResourcesKeys.SomeItemsIn___ListAreNotCorrect.Localize(SharedResourcesKeys.Product.Localize()))
                    .WithStatusCode(HttpStatusCode.BadRequest);

            Items.Remove(inventoryTransactionItemToBeRemoved);

            deletedItems.Add((
                productInstanceId,
                inventoryTransactionItemToBeRemoved.Quantity,
                inventoryTransactionItemToBeRemoved.InventoryTransactionItemUnits != null && inventoryTransactionItemToBeRemoved.InventoryTransactionItemUnits.Count > 0,
                inventoryTransactionItemToBeRemoved?.InventoryTransactionItemUnits?.Select(x => x.SerialNumber).ToList()));
        }

        if (deletedItems.Count != 0)
        {
            //RaiseEvent(new ProductsQuantityChangedEvent(Id, GetTransactionType(), deletedItems));
        }

        ////it would be set false until these updates are reflected in the stored products
        //IsTransactionProcessed = false;

        RecalculateAmount();

        return new Result<List<InventoryTransactionItem>>(Items.ToList());
    }

    //private IResult<InventoryTransactionItemUnit> RemoveUnit(string serialNumber, InventoryTransactionItem item)
    //{
    //    var unitToBeRemoved = item.InventoryTransactionItemUnits?.FirstOrDefault(x => x.SerialNumber == serialNumber);
    //    if (unitToBeRemoved == null)
    //        return new Result<InventoryTransactionItemUnit>()
    //            .WithBadRequestResult(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.Product.Localize()));

    //    item.InventoryTransactionItemUnits?.Remove(unitToBeRemoved);

    //    return new Result<InventoryTransactionItemUnit>();
    //}

    //private IResult<InventoryTransactionItemUnit> AddUnit(string serialNumber, InventoryTransactionItem item)
    //{
    //    if (item.InventoryTransactionItemUnits == null)
    //        item.InventoryTransactionItemUnitsInitializing();

    //    var doesUnitExist = item.InventoryTransactionItemUnits.Any(x => x.SerialNumber == serialNumber);
    //    if (doesUnitExist)
    //        return new Result<InventoryTransactionItemUnit>()
    //            .WithBadRequestResult(SharedResourcesKeys.DoesExist.Localize(SharedResourcesKeys.Product.Localize()));

    //    item.ADd

    //}

}
