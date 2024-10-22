using smERP.Domain.Entities.InventoryTransaction;
using smERP.Domain.Entities.Product;
using smERP.SharedKernel.Localizations.Extensions;
using smERP.SharedKernel.Localizations.Resources;
using smERP.SharedKernel.Responses;

namespace smERP.Domain.Entities.Organization;

public class StorageLocation : Entity, IAggregateRoot
{
    public int BranchId { get; private set; }
    public string Name { get; private set; } = null!;
    public virtual Branch Branch { get; private set; } = null!;
    public ICollection<StoredProductInstance> StoredProductInstances { get; private set; } = new List<StoredProductInstance>();
    public ICollection<AdjustmentTransaction> AdjustmentTransactions { get; private set; } = new List<AdjustmentTransaction>();
    public ICollection<ProcurementTransaction> ProcurementTransactions { get; private set; } = new List<ProcurementTransaction>();
    public ICollection<SalesTransaction> SalesTransactions { get; private set; } = new List<SalesTransaction>();


    private StorageLocation() { }

    internal StorageLocation(int branchId, string name)
    {
        BranchId = branchId;
        Name = name;
    }

    public IResult<List<StoredProductInstance>> AddStoredProductInstances(List<(int ProductInstnceId, int Quantity, bool IsTracked, int? ShelfLifeInDays, List<(string SerialNumber, string Status, DateOnly? ExprationDate)>? Items)> products)
    {
        if (products == null || products.Count() < 0)
            return new Result<List<StoredProductInstance>>()
                .WithError(SharedResourcesKeys.___ListMustContainAtleastOneItem.Localize(SharedResourcesKeys.Product.Localize()));

        foreach (var product in products)
        {
            var existingStoredProduct = StoredProductInstances.FirstOrDefault(x => x.ProductInstanceId == product.ProductInstnceId);
            if (existingStoredProduct != null)
            {
               var existingStoredProductUpdateResult = existingStoredProduct.Update(product.Quantity, product.ShelfLifeInDays, product.Items);
                if (existingStoredProductUpdateResult.IsFailed)
                    return existingStoredProductUpdateResult.ChangeType(new List<StoredProductInstance>());
            }
            else
            {
                var storedProductCreateResult = StoredProductInstance.Create(Id, product);
                if (storedProductCreateResult.IsFailed)
                    return storedProductCreateResult.ChangeType(new List<StoredProductInstance>());

                StoredProductInstances.Add(storedProductCreateResult.Value);
            }
        }

        return new Result<List<StoredProductInstance>>(StoredProductInstances.ToList());
    }
}
