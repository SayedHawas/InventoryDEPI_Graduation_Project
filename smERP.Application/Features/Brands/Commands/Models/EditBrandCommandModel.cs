using MediatR;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.Brands.Commands.Models;

public record EditBrandCommandModel(int BrandId, string? EnglishName, string? ArabicName) : IRequest<IResultBase>;

