using MediatR;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.Attributes.Commands.Models;

public record EditAttributeValueCommandModel(int AttributeId, int AttributeValueId, string EnglishName, string ArabicName) : IRequest<IResultBase>;