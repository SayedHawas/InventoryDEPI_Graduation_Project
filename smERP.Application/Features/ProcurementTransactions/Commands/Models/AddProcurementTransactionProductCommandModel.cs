using MediatR;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.ProcurementTransactions.Commands.Models;

public record AddProcurementTransactionProductCommandModel(int TransactionId, int ProductInstanceId, int Quantity, decimal UnitPrice,
    List<ProductItem>? UnitsToAdd) : IRequest<IResultBase>;