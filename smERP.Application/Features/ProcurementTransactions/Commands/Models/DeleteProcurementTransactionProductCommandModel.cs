using MediatR;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.ProcurementTransactions.Commands.Models;

public record DeleteProcurementTransactionProductCommandModel(int TransactionId, int ProductInstanceId) : IRequest<IResultBase>;