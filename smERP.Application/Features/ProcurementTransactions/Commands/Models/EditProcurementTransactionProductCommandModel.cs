using MediatR;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.ProcurementTransactions.Commands.Models;

public record EditProcurementTransactionProductCommandModel(int TransactionId, int ProductInstanceId, int Quantity, decimal UnitPrice,
    List<ProductItem>? UnitsToAdd,
    List<string>? UnitsToRemove) : IRequest<IResultBase>;
