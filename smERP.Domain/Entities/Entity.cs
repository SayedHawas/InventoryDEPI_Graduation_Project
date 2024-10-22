using smERP.Domain.Events;

namespace smERP.Domain.Entities;

public abstract class Entity
{
    private readonly ICollection<IDomainEvent> _events;

    protected Entity()
    {
        _events = new List<IDomainEvent>();
    }

    public int Id { get; private set; }

    public string CreatedBy { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public string ModifiedBy { get; private set; }

    public DateTime ModifiedAt { get; private set; }

    public IReadOnlyCollection<IDomainEvent> Events => _events.ToList().AsReadOnly();

    public virtual void ClearEvents() => _events.Clear();

    protected virtual void RaiseEvent(IDomainEvent domainEvent)
        => _events.Add(domainEvent);

    public override bool Equals(object? obj)
    {
        if (obj is not Entity other)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (!IsTransient() && !other.IsTransient() && Id == other.Id)
            return true;

        return false;
    }

    public static bool operator ==(Entity? a, Entity? b)
    {
        if (a is null && b is null)
            return true;

        if (a is null || b is null)
            return false;

        return a.Equals(b);
    }

    public static bool operator !=(Entity? a, Entity? b)
    {
        return !(a == b);
    }

    public override int GetHashCode()
    {
        if (!IsTransient())
        {
            return Id.GetHashCode() ^ 31;
        }
        return base.GetHashCode();
    }

    public bool IsTransient()
    {
        return Id == 0;
    }

    public void Created(string userId, DateTime date)
    {
        CreatedBy = userId;
        CreatedAt = date;
        ModifiedBy = userId;
        ModifiedAt = date;
    }

    public void Modified(string userId, DateTime date)
    {
        ModifiedBy = userId;
        ModifiedAt = date;
    }
}
