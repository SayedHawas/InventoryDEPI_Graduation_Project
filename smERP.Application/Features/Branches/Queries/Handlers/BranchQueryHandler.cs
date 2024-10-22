using MediatR;
using smERP.Application.Contracts.Persistence;
using smERP.Application.Features.Branches.Queries.Models;
using smERP.Application.Features.Branches.Queries.Responses;
using smERP.Application.Features.Brands.Queries.Responses;
using smERP.SharedKernel.Localizations.Extensions;
using smERP.SharedKernel.Localizations.Resources;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.Branches.Queries.Handlers;

public class BranchQueryHandler(IBranchRepository branchRepository) :
    IRequestHandler<GetAllBranchesQuery, IResult<IEnumerable<SelectOption>>>,
    IRequestHandler<GetPaginatedBranchesQuery, IResult<PagedResult<GetPaginatedBranchesQueryResponse>>>,
    IRequestHandler<GetBranchQuery, IResult<GetBranchQueryResponse>>,
    IRequestHandler<GetStorageLocationQuery, IResult<GetStorageLocationQueryResponse>>,
    IRequestHandler<GetBranchesWithStorageLocationsQuery, IResult<IEnumerable<GetBranchesWithStorageLocationsQueryResponse>>>,
    IRequestHandler<GetPaginatedStorageLocationsQuery, IResult<PagedResult<GetPaginatedStorageLocationsQueryResponse>>>
{
    private readonly IBranchRepository _branchRepository = branchRepository;

    public async Task<IResult<IEnumerable<SelectOption>>> Handle(GetAllBranchesQuery request, CancellationToken cancellationToken)
    {
        return new Result<IEnumerable<SelectOption>>(await _branchRepository.GetAllBranches());
    }

    public async Task<IResult<PagedResult<GetPaginatedBranchesQueryResponse>>> Handle(GetPaginatedBranchesQuery request, CancellationToken cancellationToken)
    {
        var result = await _branchRepository.GetPagedAsync(request);
        return new Result<PagedResult<GetPaginatedBranchesQueryResponse>>(result);
    }

    public async Task<IResult<GetBranchQueryResponse>> Handle(GetBranchQuery request, CancellationToken cancellationToken)
    {
        var branch = await _branchRepository.GetByIdWithStorageLocations(request.BranchId);
        if (branch == null)
            return new Result<GetBranchQueryResponse>()
                .WithBadRequestResult(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.Branch.Localize()));

        var branchResponse = new GetBranchQueryResponse(
            branch.Id, 
            branch.Name.English, 
            branch.Name.Arabic, 
            branch.StorageLocations.Select(location => new SelectOption(location.Id, location.Name)).ToList());

        return new Result<GetBranchQueryResponse>(branchResponse);
    }

    public async Task<IResult<GetStorageLocationQueryResponse>> Handle(GetStorageLocationQuery request, CancellationToken cancellationToken)
    {
        var response = await _branchRepository.GetStorageLocation(request.StorageLocationId);
        if (response == null)
            return new Result<GetStorageLocationQueryResponse>().WithNotFound();

        return new Result<GetStorageLocationQueryResponse>(response);
    }

    public async Task<IResult<IEnumerable<GetBranchesWithStorageLocationsQueryResponse>>> Handle(GetBranchesWithStorageLocationsQuery request, CancellationToken cancellationToken)
    {
        return new Result<IEnumerable<GetBranchesWithStorageLocationsQueryResponse>>(await _branchRepository.GetBranchesWithStorageLocations());
    }

    public async Task<IResult<PagedResult<GetPaginatedStorageLocationsQueryResponse>>> Handle(GetPaginatedStorageLocationsQuery request, CancellationToken cancellationToken)
    {
        var result = await _branchRepository.GetPaginatedStorageLocations(request);
        return new Result<PagedResult<GetPaginatedStorageLocationsQueryResponse>>(result);
    }
}
