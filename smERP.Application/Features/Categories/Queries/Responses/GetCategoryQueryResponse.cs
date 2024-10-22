namespace smERP.Application.Features.Categories.Queries.Responses;

public record GetCategoryQueryResponse(
    int CategoryID,
    string EnglishName,
    string ArabicName,
    int ProductUnderCategoryCount,
    int? ParentCategoryId);