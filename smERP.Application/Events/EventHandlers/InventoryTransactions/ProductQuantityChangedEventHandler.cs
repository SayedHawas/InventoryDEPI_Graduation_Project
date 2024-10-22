using MediatR;
using Microsoft.AspNetCore.SignalR;
using smERP.Application.Contracts.Persistence;
using smERP.Application.Notifications;
using smERP.Domain.Events;

namespace smERP.Application.Events.EventHandlers.InventoryTransactions;

public class ProductQuantityChangedEventHandler(IStorageLocationRepository storageLocationRepository, IBranchRepository branchRepository, INotificationRepository notificationRepository, IProductRepository productRepository, IHubContext<NotificationHub, INotificationHub> notificationHub)
    : INotificationHandler<ProductsQuantityChangedEvent>
{
    private readonly IStorageLocationRepository _storageLocationRepository = storageLocationRepository;
    private readonly IBranchRepository _branchRepository = branchRepository;
    private readonly INotificationRepository _notificationRepository = notificationRepository;
    private readonly IProductRepository _productRepository = productRepository;
    private readonly IHubContext<NotificationHub, INotificationHub> _notificationHub = notificationHub;

    public async Task Handle(ProductsQuantityChangedEvent notification, CancellationToken cancellationToken)
    {
        var storageLocation = await _storageLocationRepository.GetByID(notification.StorageLocationId);

        if (storageLocation == null) return;

        var result = storageLocation.AddStoredProductInstances(notification.ProductEntries);

        if (result.IsSuccess)
        {
            _storageLocationRepository.Update(storageLocation);

            var branch = await _branchRepository.GetByIdWithStorageLocations(storageLocation.BranchId);

            var lowProducts = new List<(int productInstanceId, bool isLow, int currentLevel, int recommendLevel)>();

            foreach (var item in notification.ProductEntries)
            {
                lowProducts.Add(branch.IsProductInstanceLowLevel(item.ProductIsntanceId));
            }

            if (lowProducts != null && lowProducts.Count > 0)
            {
                var productsName = await _productRepository.GetProductNames(lowProducts.Select(x => x.productInstanceId).ToList());

                var mergedProducts = lowProducts.Join(
                    productsName,
                    low => low.productInstanceId,
                    name => name.ProductInstanceId,
                    (low, name) => new
                    {
                        ProductInstanceId = low.productInstanceId,
                        ProductName = name.ProductInstanceName,
                        CurrentLevel = low.currentLevel,
                        RecommendLevel = low.recommendLevel
                    })
                    .ToList();

                var notifications = mergedProducts.Select(p => new Notification(branch.Id, $"Product: {p.ProductName} is in low stock, current level {p.CurrentLevel}, recommended level {p.RecommendLevel}, Branch {branch.Name.English}", "BranchManagerPolicy", NotificationType.Alert, DateTime.UtcNow, null)).ToArray();

                await _notificationRepository.AddNotifications(notifications);

                await _notificationHub.Clients.Groups($"BranchGroup_{branch.Id}").Notification(notifications);
            }
        }
    }

}

