using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using smERP.Domain.Entities;
using Newtonsoft.Json;

namespace smERP.Persistence.Data.Interceptors;

public class ChangeLogInterceptor : SaveChangesInterceptor
{
    //private readonly IHttpContextAccessor _httpContextAccessor;

    //public ChangeLogInterceptor(IHttpContextAccessor httpContextAccessor)
    //{
    //    _httpContextAccessor = httpContextAccessor;
    //}

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        LogChanges(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        LogChanges(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void LogChanges(DbContext? context)
    {
        if (context == null) return;

        var changeLogs = new List<ChangeLog>();
        var userId = "1";//_httpContextAccessor.HttpContext?.User?.Identity?.Name;

        var serializerSettings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            MaxDepth = 3
        };

        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.Entity is Entity entity && (entry.State == EntityState.Added || entry.State == EntityState.Modified || entry.State == EntityState.Deleted))
            {
                string? changes = null;
                if (entry.State != EntityState.Deleted)
                {
                    var changedProperties = entry.Properties
                        .Where(p => p.IsModified || entry.State == EntityState.Added)
                        .ToDictionary(p => p.Metadata.Name, p => p.CurrentValue);



                    changes = JsonConvert.SerializeObject(changedProperties, serializerSettings);
                }

                var changeLog = new ChangeLog
                {
                    EntityId = entity.Id,
                    EntityName = entity.GetType().Name,
                    Action = entry.State.ToString(),
                    Changes = changes ?? "Null",
                    Timestamp = DateTime.UtcNow,
                    UserId = userId
                };
                changeLogs.Add(changeLog);
            }
        }

        if (changeLogs.Count != 0)
        {
            context.AddRange(changeLogs);
        }
    }
}
