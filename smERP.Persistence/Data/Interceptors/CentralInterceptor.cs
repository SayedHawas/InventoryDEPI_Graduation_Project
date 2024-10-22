using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Newtonsoft.Json;
using smERP.Domain.Entities;
using smERP.Persistence.Outbox;
using System.Collections.Concurrent;

namespace smERP.Persistence.Data.Interceptors;

public class CentralInterceptor : SaveChangesInterceptor
{
    private readonly ConcurrentDictionary<Type, bool> _isSoftDeleteCache = new();
    private readonly JsonSerializerSettings _serializerSettings;

    public CentralInterceptor()
    {
        _serializerSettings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            MaxDepth = 3
        };
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        ProcessChanges(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        ProcessChanges(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void ProcessChanges(DbContext? context)
    {
        if (context == null) return;

        var changeLogs = new List<ChangeLog>();
        var userId = "1";
        var currentDateTime = DateTime.UtcNow;

        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.Entity is Entity entity)
            {
                ProcessSoftDelete(entry);
                ProcessAuditFields(entry, entity, userId, currentDateTime);
                ProcessChangeLog(entry, entity, changeLogs, userId);
            }
        }

        if (changeLogs.Count > 0)
        {
            context.AddRange(changeLogs);
        }

        ConvertEventsToMessages(context);
    }

    private void ProcessSoftDelete(EntityEntry entry)
    {
        if (IsSoftDeleteEntity(entry.Entity.GetType()))
        {
            var deletableEntity = (ISoftDelete)entry.Entity;
            switch (entry.State)
            {
                case EntityState.Added:
                    deletableEntity.IsDeleted = false;
                    break;
                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    deletableEntity.IsDeleted = true;
                    deletableEntity.DeletedAt = DateTime.UtcNow;
                    break;
            }
        }
    }

    private void ProcessAuditFields(EntityEntry entry, Entity entity, string userId, DateTime currentDateTime)
    {
        switch (entry.State)
        {
            case EntityState.Added:
                entity.Created(userId, currentDateTime);
                break;
            case EntityState.Modified:
                entity.Modified(userId, currentDateTime);
                break;
        }
    }

    private void ProcessChangeLog(EntityEntry entry, Entity entity, List<ChangeLog> changeLogs, string userId)
    {
        if (entry.State == EntityState.Added || entry.State == EntityState.Modified || entry.State == EntityState.Deleted)
        {
            string? changes = null;
            if (entry.State != EntityState.Deleted)
            {
                var changedProperties = entry.Properties
                    .Where(p => p.IsModified || entry.State == EntityState.Added)
                    .ToDictionary(p => p.Metadata.Name, p => p.CurrentValue);
                changes = JsonConvert.SerializeObject(changedProperties, _serializerSettings);
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

    private void ConvertEventsToMessages(DbContext context)
    {
        var events = context.ChangeTracker
            .Entries<Entity>()
            .Select(x => x.Entity)
            .SelectMany(entity =>
            {
                var domainEvents = entity.Events;

                entity.ClearEvents();

                return domainEvents;
            })
            .Select(domainEvent => new OutboxMessage
            {
                OccuredOnUtc = DateTime.UtcNow,
                Type = domainEvent.GetType().Name,
                Contect = JsonConvert.SerializeObject(
                    domainEvent,
                    new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All
                    })

            })
            .ToList();

        context.Set<OutboxMessage>().AddRange(events);
    }
    private bool IsSoftDeleteEntity(Type type)
    {
        return _isSoftDeleteCache.GetOrAdd(type, t => typeof(ISoftDelete).IsAssignableFrom(t));
    }

}