using MediatR;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.ProcurementTransactions.Commands.Models;

public record ProductItem(string SerialNumber, DateOnly? ExpirationDate);

public record ProductEntry(int ProductInstanceId, int Quantity, decimal UnitPrice, List<ProductItem>? Units);

public record Payment(decimal PayedAmount, string PaymentMethod);

public record AddProcurementTransactionCommandModel(
    int BranchId,
    int StorageLocationId,
    int SupplierId,
    DateTime? TransactionDate,
    Payment? Payment,
    List<ProductEntry> Products) : IRequest<IResultBase>;