using MediatR;
using smERP.Application.Features.Branches.Queries.Responses;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.Branches.Queries.Models;

public record GetPaginatedStorageLocationsQuery(int BranchId, PaginationParameters PaginationParameters)
    : IRequest<IResult<PagedResult<GetPaginatedStorageLocationsQueryResponse>>>
{
    public int PageNumber => PaginationParameters.PageNumber;
    public int PageSize => PaginationParameters.PageSize;
    public string? SortBy => PaginationParameters.SortBy;
    public bool SortDescending => PaginationParameters.SortDescending;
    public string? SearchTerm => PaginationParameters.SearchTerm;
    public DateTime? StartDate => PaginationParameters.StartDate;
    public DateTime? EndDate => PaginationParameters.EndDate;
}
