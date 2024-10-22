using MediatR;
using smERP.Application.Contracts.Persistence;
using smERP.Application.Features.ProcurementTransactions.Queries.Models;
using smERP.Application.Features.ProcurementTransactions.Queries.Responses;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.ProcurementTransactions.Queries.Handlers;

public class ProcurementTransactionQueryHandler(IProcurementTransactionRepository procurementTransactionRepository) :
    IRequestHandler<GetPaginatedProcurementTransactionQuery, IResult<PagedResult<GetPaginatedProcurementTransactionQueryResponse>>>,
    IRequestHandler<GetProcurementTransactionPaymentQuery, IResult<GetProcurementTransactionPaymentQueryResponse>>,
    IRequestHandler<GetProcurementTransactionProductQuery, IResult<GetProcurementTransactionProductQueryResponse>>
{
    private readonly IProcurementTransactionRepository _procurementTransactionRepository = procurementTransactionRepository;

    public async Task<IResult<PagedResult<GetPaginatedProcurementTransactionQueryResponse>>> Handle(GetPaginatedProcurementTransactionQuery request, CancellationToken cancellationToken)
    {
        var paginatedTransactions = await _procurementTransactionRepository.GetPagedAsync(request);
        return new Result<PagedResult<GetPaginatedProcurementTransactionQueryResponse>>(paginatedTransactions);
    }

    public async Task<IResult<GetProcurementTransactionPaymentQueryResponse>> Handle(GetProcurementTransactionPaymentQuery request, CancellationToken cancellationToken)
    {
        var payment = await _procurementTransactionRepository.GetTransactionPayment(request.TransactionId, request.PaymentId);
        if (payment == null) return new Result<GetProcurementTransactionPaymentQueryResponse>().WithNotFound();
        return new Result<GetProcurementTransactionPaymentQueryResponse>(payment);
    }

    public async Task<IResult<GetProcurementTransactionProductQueryResponse>> Handle(GetProcurementTransactionProductQuery request, CancellationToken cancellationToken)
    {
        var product = await _procurementTransactionRepository.GetTransactionProduct(request.TransactionId, request.ProductInstanceId);
        if (product == null) return new Result<GetProcurementTransactionProductQueryResponse>().WithNotFound();
        return new Result<GetProcurementTransactionProductQueryResponse>(product);
    }
}
