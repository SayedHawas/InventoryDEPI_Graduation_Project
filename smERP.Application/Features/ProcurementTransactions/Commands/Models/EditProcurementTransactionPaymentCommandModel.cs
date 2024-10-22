using MediatR;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.ProcurementTransactions.Commands.Models;

public record EditProcurementTransactionPaymentCommandModel(
    int TransactionId,
    int PaymentId,
    decimal PayedAmount,
    string PaymentMethod) : IRequest<IResultBase>;