using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Newtonsoft.Json;
using smERP.Domain.Entities;
using smERP.Persistence.Outbox;

namespace smERP.Persistence.Data.Interceptors;

public class ConvertDomainEventsToOutboxMessagesInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void ConvertEventsToMessages(DbContext? context)
    {
        if (context == null) return;

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
}
