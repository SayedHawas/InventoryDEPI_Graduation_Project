using MediatR;
using smERP.Application.Features.ProcurementTransactions.Queries.Responses;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.ProcurementTransactions.Queries.Models;

public record GetPaginatedProcurementTransactionQuery() : PaginationParameters, IRequest<IResult<PagedResult<GetPaginatedProcurementTransactionQueryResponse>>>;