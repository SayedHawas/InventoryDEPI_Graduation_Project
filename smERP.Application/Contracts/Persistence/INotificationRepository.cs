
using MediatR;
using smERP.Application.Notifications;
using System.Security.Claims;

namespace smERP.Application.Contracts.Persistence;

public interface INotificationRepository
{
    Task AddNotification(Notification notification);
    Task AddNotifications(IEnumerable<Notification> notifications);
    Task<List<Notification>> GetNotifications();
    Task<List<Notification>> GetNotificationsById(List<int> notificationIds);
    Task<List<Notification>> GetNotificationsForUser(ClaimsPrincipal user);
    Task<Notification?> GetNotification(int notificationId);
    void DeleteNotification(Notification notification);
    void UpdateNotification(Notification notification);
    void UpdateNotifications(IEnumerable<Notification> notifications);
}
