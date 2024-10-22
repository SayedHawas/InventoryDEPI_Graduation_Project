using MediatR;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.Products.Commands.Models;

public record DeleteProductCommandModel(int ProductId) : IRequest<IResultBase>;
