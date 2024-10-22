using MediatR;
using smERP.Application.Features.Products.Queries.Responses;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.Products.Queries.Models;

public record GetProductQuery(int ProductId) : IRequest<IResult<GetProductQueryResponse>>;
