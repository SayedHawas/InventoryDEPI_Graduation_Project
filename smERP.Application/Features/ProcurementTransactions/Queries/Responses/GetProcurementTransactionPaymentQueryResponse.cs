namespace smERP.Application.Features.ProcurementTransactions.Queries.Responses;

public record GetProcurementTransactionPaymentQueryResponse(int PaymentId, decimal PayedAmount, string PaymentMethod);