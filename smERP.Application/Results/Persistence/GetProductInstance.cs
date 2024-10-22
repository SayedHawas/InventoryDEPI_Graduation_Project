namespace smERP.Application.Results.Persistence;

public record GetProductInstance(int ProductId, int ProductInstanceId, bool IsTracked, int? ShelfLifeInDays);