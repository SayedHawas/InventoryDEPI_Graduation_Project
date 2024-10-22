using MediatR;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.Categories.Commands.Models;

public record EditCategoryCommandModel(int CategoryId, string? ArabicName, string? EnglishName, int? ParentCategoryId) : IRequest<IResultBase>;

