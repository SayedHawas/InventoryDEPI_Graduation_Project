using MediatR;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.ProcurementTransactions.Commands.Models;

public record DeleteProcurementTransactionCommandModel(int ProcurementTransactionId) : IRequest<IResultBase>;