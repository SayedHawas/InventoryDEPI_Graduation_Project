using smERP.Application.Features.Branches.Queries.Models;

namespace smERP.Application.Features.Categories.Queries.Responses;

public record GetPaginatedCategoriesQueryResponse(
    int CategoryID,
    string EnglishName,
    string ArabicName,
    int ProductUnderCategoryCount,
    SelectOption? ParentCategory,
    List<SelectOption> SubCategories);

