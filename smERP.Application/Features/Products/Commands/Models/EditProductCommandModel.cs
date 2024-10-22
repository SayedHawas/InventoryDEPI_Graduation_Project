using MediatR;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.Products.Commands.Models;

public record EditProductCommandModel(
    int ProductId,
    string? EnglishName,
    string? ArabicName,
    string? ModelNumber,
    int? BrandId,
    int? CategoryId,
    string? Description,
    int? ShelfLifeInDays,
    int? WarrantyInDays) : IRequest<IResultBase>;
