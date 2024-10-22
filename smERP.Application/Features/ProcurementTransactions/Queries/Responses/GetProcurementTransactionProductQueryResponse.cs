using smERP.Application.Features.ProcurementTransactions.Commands.Models;

namespace smERP.Application.Features.ProcurementTransactions.Queries.Responses;

public record GetProcurementTransactionProductQueryResponse(int ProductInstanceId, int Quantity, decimal UnitPrice, bool IsTracked, int? ShelfLifeInDays, IEnumerable<string>? Units);