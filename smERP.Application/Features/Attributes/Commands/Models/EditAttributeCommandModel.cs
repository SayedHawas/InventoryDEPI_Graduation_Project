using MediatR;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.Attributes.Commands.Models;

public record EditAttributeCommandModel(int AttributeId, string? EnglishName, string? ArabicName, List<AddAttributeValueModel>? ValuesToAdd, List<EditAttributeValueModel>? ValuesToEdit) : IRequest<IResultBase>;

public record EditAttributeValueModel(int AttributeValueId, string? EnglishName, string? ArabicName);

public record AddAttributeValueModel(string EnglishName, string ArabicName);