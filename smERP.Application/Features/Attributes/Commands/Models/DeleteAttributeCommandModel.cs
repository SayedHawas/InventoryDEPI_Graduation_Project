using MediatR;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.Attributes.Commands.Models;

public record DeleteAttributeCommandModel(int AttributeId) : IRequest<IResultBase>;
