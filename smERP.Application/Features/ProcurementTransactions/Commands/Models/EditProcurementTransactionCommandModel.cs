using MediatR;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.ProcurementTransactions.Commands.Models;

public record Unit(string SerialNumber, DateOnly? ExpirationDate);

public record EditProcurementTransactionCommandModel(int TransactionId, int? SupplierId, List<ItemUpdate>? ItemUpdates, List<int>? ItemsToRemove, List<NewItem>? NewItems, List<PaymentUpdate>? PaymentUpdates, List<Payment>? NewPayments, List<int> PaymentsToRemove) : IRequest<IResultBase>;

public record ItemUpdate(int ProductInstanceId, decimal? UnitPrice, int? Quantity, UnitUpdates? UnitUpdates);

public record UnitUpdates(List<Unit>? ToAdd, List<string>? ToRemove);

public record NewItem(int ProductInstanceId, decimal UnitPrice, int Quantity, List<Unit>? Units);

public record PaymentUpdate(int PaymentTransactionId, decimal PayedAmount, string PaymentMethod);