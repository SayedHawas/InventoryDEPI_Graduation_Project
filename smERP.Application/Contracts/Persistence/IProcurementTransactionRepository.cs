using smERP.Application.Features.ProcurementTransactions.Queries.Responses;
using smERP.Domain.Entities.InventoryTransaction;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Contracts.Persistence;

public interface IProcurementTransactionRepository : IRepository<ProcurementTransaction>
{
    new Task<PagedResult<GetPaginatedProcurementTransactionQueryResponse>> GetPagedAsync(PaginationParameters parameters);
    Task<GetProcurementTransactionPaymentQueryResponse?> GetTransactionPayment(int TransactionId, int PaymentId);
    Task<GetProcurementTransactionProductQueryResponse?> GetTransactionProduct(int TransactionId, int ProductId);
}
