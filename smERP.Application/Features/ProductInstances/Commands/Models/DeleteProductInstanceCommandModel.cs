using MediatR;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.ProductInstances.Commands.Models;

public record DeleteProductInstanceCommandModel(int ProductId, int ProductInstanceId) : IRequest<IResultBase>;