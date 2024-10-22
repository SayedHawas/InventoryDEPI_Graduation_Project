using MediatR;
using smERP.SharedKernel.Responses;

namespace smERP.Application.Features.Attributes.Commands.Models;

public record AddAttributeCommandModel(string EnglishName, string ArabicName, List<AttributeValueModel> Values) : IRequest<IResultBase>;

public record AttributeValueModel(string EnglishName, string ArabicName);
