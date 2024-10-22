
namespace smERP.Application.Notifications;

public interface INotificationHub
{
    Task Notification(Notification notification);
    Task Notification(IEnumerable<Notification> notifications);
    Task NotificationRead(List<int> notificationIds);
}