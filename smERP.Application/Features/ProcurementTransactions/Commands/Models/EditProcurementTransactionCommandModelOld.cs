using MediatR;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.ProcurementTransactions.Commands.Models;

public record EditProcurementTransactionCommandModelOld(int ProcurementTransactionId, int? SupplierId, List<ProductEntry>? Products, List<PaymentUpdate>? Payments) : IRequest<IResultBase>;

