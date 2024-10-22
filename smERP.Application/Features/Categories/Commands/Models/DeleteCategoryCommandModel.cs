using MediatR;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.Categories.Commands.Models;

public record DeleteCategoryCommandModel(int CategoryID) : IRequest<IResultBase>;
