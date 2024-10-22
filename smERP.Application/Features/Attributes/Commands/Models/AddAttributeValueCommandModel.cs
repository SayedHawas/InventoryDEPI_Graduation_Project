using MediatR;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.Attributes.Commands.Models;

public record AddAttributeValueCommandModel(int AttributeId, string EnglishName, string ArabicName) : IRequest<IResultBase>;
