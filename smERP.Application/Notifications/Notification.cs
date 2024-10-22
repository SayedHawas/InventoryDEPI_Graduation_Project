
namespace smERP.Application.Notifications;

public class Notification : IGenericNotification
{
    public int Id { get; set; }
    public int? BranchId { get; set; }
    public string Message { get; set; } = null!;
    public string UserPolicy { get; set; } = null!;
    public NotificationLevel Level { get; set; }
    public NotificationType Type { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ReadAt { get; set; }

    public Notification(int? branchId, string message, string userPolicy, NotificationType type, DateTime createdAt, DateTime? readAt)
    {
        BranchId = branchId;
        Message = message;
        UserPolicy = userPolicy;
        Type = type;
        CreatedAt = createdAt;
        ReadAt = readAt;
    }
}
