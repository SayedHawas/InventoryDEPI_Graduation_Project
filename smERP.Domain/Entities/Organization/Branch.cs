
using smERP.Domain.Entities.Product;
using smERP.Domain.ValueObjects;
using smERP.SharedKernel.Localizations.Extensions;
using smERP.SharedKernel.Localizations.Resources;
using smERP.SharedKernel.Responses;
using System.Net;

namespace smERP.Domain.Entities.Organization;

public class Branch : Entity, IAggregateRoot
{
    public BilingualName Name { get; private set; } = null!;
    public ICollection<StorageLocation> StorageLocations { get; private set; } = null!;
    public ICollection<BranchProductInstanceAlertLevel> BranchProductInstanceAlertLevels { get; private set; } = [];

    private Branch(BilingualName name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    private Branch() { }

    public static IResult<Branch> Create(string englishName, string arabicName)
    {
        var nameResult = BilingualName.Create(englishName, arabicName);
        if (nameResult.IsFailed)
            return nameResult.ChangeType(new Branch());

        return new Result<Branch>(new Branch(nameResult.Value));
    }

    public IResult<StorageLocation> AddStorageLocation(string name)
    {
        if (string.IsNullOrEmpty(name))
            return new Result<StorageLocation>()
                .WithError(SharedResourcesKeys.Required_FieldName.Localize(SharedResourcesKeys.Name.Localize()))
                .WithStatusCode(HttpStatusCode.BadRequest);

        var storageLocationToBeCreated = new StorageLocation(Id, name);

        if (StorageLocations == null)
        {
            StorageLocations = [storageLocationToBeCreated];
            return new Result<StorageLocation>(storageLocationToBeCreated);
        }

        if (StorageLocations.Any(av => av.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
        {
            return new Result<StorageLocation>()
                .WithError(SharedResourcesKeys.DoesExist.Localize(SharedResourcesKeys.Name.Localize()))
                .WithStatusCode(HttpStatusCode.BadRequest);
        }

        StorageLocations.Add(storageLocationToBeCreated);

        return new Result<StorageLocation>(storageLocationToBeCreated);
    }

    public IResult<StorageLocation> UpdateStorageLocation(int storageLocationId, string name)
    {
        if (string.IsNullOrEmpty(name))
            return new Result<StorageLocation>()
                .WithError(SharedResourcesKeys.Required_FieldName.Localize(SharedResourcesKeys.Name.Localize()))
                .WithStatusCode(HttpStatusCode.BadRequest);

        if (storageLocationId < 0)
            return new Result<StorageLocation>()
                .WithError(SharedResourcesKeys.___MustBeAPositiveNumber.Localize(SharedResourcesKeys.StorageLocation.Localize()))
                .WithStatusCode(HttpStatusCode.BadRequest);

        var storageLocationToBeUpdated = StorageLocations.FirstOrDefault(x => x.Id == storageLocationId);
        if (storageLocationToBeUpdated == null)
            return new Result<StorageLocation>()
                .WithError(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.StorageLocation.Localize()))
                .WithStatusCode(HttpStatusCode.BadRequest);

        if (StorageLocations.Any(av => av.Name.Equals(storageLocationToBeUpdated.Name, StringComparison.OrdinalIgnoreCase)))
        {
            return new Result<StorageLocation>()
                .WithError(SharedResourcesKeys.DoesExist.Localize(SharedResourcesKeys.Name.Localize()))
                .WithStatusCode(HttpStatusCode.BadRequest);
        }

        return new Result<StorageLocation>(storageLocationToBeUpdated);
    }

    public IResult<StorageLocation> RemoveStorageLocation(int storageLocationId)
    {
        if (storageLocationId < 0)
            return new Result<StorageLocation>()
                .WithError(SharedResourcesKeys.___MustBeAPositiveNumber.Localize(SharedResourcesKeys.StorageLocation.Localize()))
                .WithStatusCode(HttpStatusCode.BadRequest);

        var storageLocationToBeDeleted = StorageLocations.FirstOrDefault(x => x.Id == storageLocationId);
        if (storageLocationToBeDeleted == null)
            return new Result<StorageLocation>()
                .WithError(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.StorageLocation.Localize()))
                .WithStatusCode(HttpStatusCode.BadRequest);

        StorageLocations.Remove(storageLocationToBeDeleted);

        return new Result<StorageLocation>(storageLocationToBeDeleted);
    }

    public IResult<BranchProductInstanceAlertLevel> SetProductInstanceAlertLevel(int productInstanceId, int alertLevel)
    {
        if (productInstanceId < 0)
            return new Result<BranchProductInstanceAlertLevel>()
                .WithError(SharedResourcesKeys.___MustBeAPositiveNumber.Localize(SharedResourcesKeys.Product.Localize()))
                .WithStatusCode(HttpStatusCode.BadRequest);

        if (alertLevel < 0)
            return new Result<BranchProductInstanceAlertLevel>()
                .WithError(SharedResourcesKeys.___MustBeAPositiveNumber.Localize(SharedResourcesKeys.Quantity.Localize()))
                .WithStatusCode(HttpStatusCode.BadRequest);

        var existingAlertLevel = BranchProductInstanceAlertLevels.FirstOrDefault(x => x.ProductInstanceId == productInstanceId);
        if (existingAlertLevel != null)
        {
            existingAlertLevel.AlertLevel = alertLevel;
            return new Result<BranchProductInstanceAlertLevel>(existingAlertLevel);
        }

        var newAlertLevel = new BranchProductInstanceAlertLevel(Id, productInstanceId, alertLevel);
        BranchProductInstanceAlertLevels.Add(newAlertLevel);

        return new Result<BranchProductInstanceAlertLevel>(newAlertLevel);
    }

    public (int ProductInstanceId, bool IsLow, int CurrentLevel, int RecommendLevel) IsProductInstanceLowLevel(int productInstanceId)
    {
        var productInstanceAlertLevel = BranchProductInstanceAlertLevels.FirstOrDefault(x => x.ProductInstanceId == productInstanceId);
        if (productInstanceAlertLevel == null) return (productInstanceId, false, 0, 0);

        var productInstanceQuantity = StorageLocations
            .SelectMany(sl => sl.StoredProductInstances)
            .Where(x => x.ProductInstanceId == productInstanceId)
            .Sum(spi => spi.Quantity);

        return (productInstanceId, productInstanceAlertLevel.AlertLevel > productInstanceQuantity, productInstanceQuantity, productInstanceAlertLevel.AlertLevel);
    }

    public IEnumerable<BranchProductSummary> GetBranchProductSummaries()
    {
        var productSummaries = StorageLocations
            .SelectMany(sl => sl.StoredProductInstances)
            .GroupBy(spi => spi.ProductInstanceId)
            .Select(group => new BranchProductSummary
            {
                ProductInstanceId = group.Key,
                TotalQuantity = group.Sum(spi => spi.Quantity),
                AlertLevel = BranchProductInstanceAlertLevels
                    .FirstOrDefault(al => al.ProductInstanceId == group.Key)?.AlertLevel
            })
            .ToList();

        return productSummaries;
    }
}
public class BranchProductSummary
{
    public int ProductInstanceId { get; set; }
    public int TotalQuantity { get; set; }
    public int? AlertLevel { get; set; }
    public bool IsAlertLevelReached => AlertLevel.HasValue && TotalQuantity <= AlertLevel.Value;
}
