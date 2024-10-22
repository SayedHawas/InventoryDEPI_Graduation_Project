using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using smERP.Application.Contracts.Persistence;
using System.Security.Claims;

namespace smERP.Application.Notifications;

public class NotificationHub(INotificationRepository notificationRepository, IUnitOfWork unitOfWork, IAuthorizationService authorizationService) : Hub<INotificationHub>
{
    private readonly INotificationRepository _notificationRepository = notificationRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IAuthorizationService _authorizationService = authorizationService;

    public override async Task OnConnectedAsync()
    {
        if (await IsUserAdmin(Context.User))
        {
            await JoinAdminGroup();
        }

        var branchClaim = Context.User.Claims.FirstOrDefault(c => c.Type == "branch");
        if (branchClaim != null && int.TryParse(branchClaim.Value, out int branchId))
        {
            await JoinBranchGroup(branchId);
        }
        else
        {
            Context.Abort();
            return;
        }

        await base.OnConnectedAsync();

        var userNotifications = await _notificationRepository.GetNotificationsForUser(Context.User);

        await Clients.Caller.Notification(userNotifications);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await LeaveAdminGroup();

        var branchClaim = Context.User.Claims.FirstOrDefault(c => c.Type == "branch");
        if (branchClaim != null && int.TryParse(branchClaim.Value, out int branchId))
        {
            await LeaveBranchGroup(branchId);
        }

        await base.OnDisconnectedAsync(exception);
    }

    private async Task JoinAdminGroup()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "AdminGroup");
    }

    private async Task JoinBranchGroup(int branchId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"BranchGroup_{branchId}");
    }

    private async Task LeaveAdminGroup()
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, "AdminGroup");
    }

    private async Task LeaveBranchGroup(int branchId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"BranchGroup_{branchId}");
    }

    private async Task<bool> IsUserAdmin(ClaimsPrincipal user)
    {
        var result = await _authorizationService.AuthorizeAsync(user, null, "AdminPolicy");
        return result.Succeeded;
    }
    public async Task SendNotification(List<string> Groups,Notification notification)
    {
        await _notificationRepository.AddNotification(notification);
        await _unitOfWork.SaveChangesAsync();
        await Clients.Groups(Groups).Notification(notification);
    }

    public async Task SendNotifications(List<string> Groups, IEnumerable<Notification> notifications)
    {
        await _notificationRepository.AddNotifications(notifications);
        await _unitOfWork.SaveChangesAsync();
        await Clients.Groups(Groups).Notification(notifications);
    }

    public async Task NotificationsRead(List<int> notificationIds)
    {
        var notifications = await _notificationRepository.GetNotificationsById(notificationIds);
        if (notifications is not null && notifications.Count == notificationIds.Count)
        {
            foreach (var notification in notifications)
            {
                notification.ReadAt = DateTime.UtcNow;
            }

            _notificationRepository.UpdateNotifications(notifications);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
