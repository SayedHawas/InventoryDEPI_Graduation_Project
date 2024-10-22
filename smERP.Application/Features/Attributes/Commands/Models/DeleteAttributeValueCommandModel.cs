using MediatR;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.Attributes.Commands.Models;

public record DeleteAttributeValueCommandModel(int AttributeId, int AttributeValueId) : IRequest<IResultBase>;