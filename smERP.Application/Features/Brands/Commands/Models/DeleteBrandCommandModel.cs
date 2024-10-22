using MediatR;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.Brands.Commands.Models;

public record DeleteBrandCommandModel(int BrandId) : IRequest<IResultBase>;
