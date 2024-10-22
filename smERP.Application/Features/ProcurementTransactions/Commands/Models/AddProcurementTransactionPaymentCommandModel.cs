using MediatR;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.ProcurementTransactions.Commands.Models;

public record AddProcurementTransactionPaymentCommandModel(
    int TransactionId,
    decimal PayedAmount,
    string PaymentMethod) : IRequest<IResultBase>;