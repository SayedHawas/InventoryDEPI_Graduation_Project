using MediatR;
using smERP.Application.Contracts.Persistence;
using smERP.Application.Features.Branches.Queries.Models;
using smERP.Application.Features.Categories.Queries.Models;
using smERP.Application.Features.Categories.Queries.Responses;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.Categories.Queries.Handlers;

public class CategoriesQueryHandler(ICategoryRepository categoryRepository) :
    IRequestHandler<GetCategoryQuery, IResult<GetCategoryQueryResponse>>,
    IRequestHandler<GetPaginatedCategoriesQuery, IResult<PagedResult<GetPaginatedCategoriesQueryResponse>>>,
    IRequestHandler<GetProductCategoriesQuery, IResult<IEnumerable<SelectOption>>>,
    IRequestHandler<GetParentCategoriesQuery, IResult<IEnumerable<SelectOption>>>
{
    private readonly ICategoryRepository _categoryRepository = categoryRepository;

    public async Task<IResult<GetCategoryQueryResponse>> Handle(GetCategoryQuery request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByID(request.CategoryId);

        if(category == null)
            return new Result<GetCategoryQueryResponse>().WithNotFound();

        var categoryResponse = new GetCategoryQueryResponse(category.Id, category.Name.English, category.Name.Arabic, category.ProductCount, category.ParentCategoryId);
        return new Result<GetCategoryQueryResponse>(categoryResponse);
    }

    public async Task<IResult<PagedResult<GetPaginatedCategoriesQueryResponse>>> Handle(GetPaginatedCategoriesQuery request, CancellationToken cancellationToken)
    {
        var paginatedCategories = await _categoryRepository.GetPagedAsync(request);
        return new Result<PagedResult<GetPaginatedCategoriesQueryResponse>>(paginatedCategories);
    }

    public async Task<IResult<IEnumerable<SelectOption>>> Handle(GetProductCategoriesQuery request, CancellationToken cancellationToken)
    {
        var productCategories = await _categoryRepository.GetProductCategories();
        return new Result<IEnumerable<SelectOption>>(productCategories);
    }

    public async Task<IResult<IEnumerable<SelectOption>>> Handle(GetParentCategoriesQuery request, CancellationToken cancellationToken)
    {
        var parentCategories = await _categoryRepository.GetParentCategories();
        return new Result<IEnumerable<SelectOption>>(parentCategories);
    }
}
