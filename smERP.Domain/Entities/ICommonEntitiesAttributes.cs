namespace smERP.Domain.Entities;

public interface ICommonEntitiesAttributes
{
    public DateTime CreatedAt { get; init; }
    public int CreatedBy { get; init; }
    public DateTime UpdatedAt { get; init; }
    public int UpdatedBy { get; init; }
}

