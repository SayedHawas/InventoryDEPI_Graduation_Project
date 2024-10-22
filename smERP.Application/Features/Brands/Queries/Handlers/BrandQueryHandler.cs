using MediatR;
using smERP.Application.Contracts.Persistence;
using smERP.Application.Features.Branches.Queries.Models;
using smERP.Application.Features.Brands.Queries.Models;
using smERP.Application.Features.Brands.Queries.Responses;
using smERP.SharedKernel.Localizations.Extensions;
using smERP.SharedKernel.Localizations.Resources;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.Brands.Queries.Handlers;

public class BrandQueryHandler(IBrandRepository brandRepository) :
    IRequestHandler<GetPaginatedBrandsQuery, IResult<PagedResult<GetPaginatedBrandsQueryResponse>>>,
    IRequestHandler<GetBrandQuery, IResult<GetBrandQueryResponse>>,
    IRequestHandler<GetBrandsQuery, IResult<IEnumerable<SelectOption>>>
{
    private readonly IBrandRepository _brandRepository = brandRepository;

    public async Task<IResult<PagedResult<GetPaginatedBrandsQueryResponse>>> Handle(GetPaginatedBrandsQuery request, CancellationToken cancellationToken)
    {
        var brands = await _brandRepository.GetPagedAsync(request);

        return new Result<PagedResult<GetPaginatedBrandsQueryResponse>>(brands);
    }

    public async Task<IResult<GetBrandQueryResponse>> Handle(GetBrandQuery request, CancellationToken cancellationToken)
    {
        var brand = await _brandRepository.GetByID(request.BrandId);
        if (brand == null)
            return new Result<GetBrandQueryResponse>()
                .WithBadRequestResult(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.Brand.Localize()));

        return new Result<GetBrandQueryResponse>(new GetBrandQueryResponse(brand.Id, brand.Name.English, brand.Name.Arabic));
    }

    public async Task<IResult<IEnumerable<SelectOption>>> Handle(GetBrandsQuery request, CancellationToken cancellationToken)
    {
        return new Result<IEnumerable<SelectOption>>(await _brandRepository.GetBrands());
    }
}
