
namespace smERP.Application.Notifications;

public interface IGenericNotification
{
    public int Id { get; set; }
    public int? BranchId { get; set; }
    public string Message { get; set; }
    public string UserPolicy { get; set; }
    public NotificationType Type { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ReadAt { get; set; }
}
