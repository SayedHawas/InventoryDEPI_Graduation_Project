using MediatR;
using smERP.Application.Features.Auth.Queries.Responses;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.Auth.Queries.Models;

public record GetUserQuery(string userId) : IRequest<IResult<GetUserQueryResponse>>;