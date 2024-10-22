using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using smERP.Application.Contracts.Persistence;
using smERP.Application.Notifications;
using smERP.Persistence.Data;
using System.Security.Claims;

namespace smERP.Persistence.Repositories;

public class NotificationRepository(ProductDbContext context, IAuthorizationService authorizationService) : INotificationRepository
{
    private readonly ProductDbContext _context = context;
    private readonly IAuthorizationService _authorizationService = authorizationService;

    public async Task AddNotification(Notification notification)
    {
        await _context.Notifications.AddAsync(notification);
    }

    public async Task AddNotifications(IEnumerable<Notification> notifications)
    {
        await _context.Notifications.AddRangeAsync(notifications);
    }

    public void DeleteNotification(Notification notification)
    {
        _context.Notifications.Remove(notification);
    }

    public async Task<Notification?> GetNotification(int notificationId)
    {
        return await _context.Notifications.FirstOrDefaultAsync(x => x.Id == notificationId);
    }

    public async Task<List<Notification>> GetNotifications()
    {
        return await _context.Notifications.ToListAsync();
    }

    public async Task<List<Notification>> GetNotificationsById(List<int> notificationIds)
    {
        return await _context.Notifications.Where(x => notificationIds.Contains(x.Id)).ToListAsync();
    }

    public async Task<List<Notification>> GetNotificationsForUser(ClaimsPrincipal user)
    {
        var notifications = _context.Notifications.AsQueryable();
        var userNotifications = new List<Notification>();

        if ((await _authorizationService.AuthorizeAsync(user, null, "AdminPolicy")).Succeeded)
        {
            return await notifications.ToListAsync();
        }

        var isConverted = int.TryParse(user.Claims.FirstOrDefault(c => c.Type == "branch")?.Value, out int branchId);

        if (!isConverted) return [];

        if ((await _authorizationService.AuthorizeAsync(user, null, "BranchManagerPolicy")).Succeeded)
        {
            userNotifications = await notifications
                .Where(n => n.BranchId == branchId)
                .ToListAsync();
        }
        else if ((await _authorizationService.AuthorizeAsync(user, null, "BranchAccessPolicy")).Succeeded)
        {
            userNotifications = await notifications
                .Where(n => n.BranchId.HasValue && n.BranchId.Value == branchId)
                .ToListAsync();
        }

        return userNotifications;
    }

    public void UpdateNotification(Notification notification)
    {
        _context.Notifications.Update(notification);
    }

    public void UpdateNotifications(IEnumerable<Notification> notifications)
    {
        _context.Notifications.UpdateRange(notifications);
    }
}
