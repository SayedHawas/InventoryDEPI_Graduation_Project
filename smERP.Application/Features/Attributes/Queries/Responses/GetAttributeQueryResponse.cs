namespace smERP.Application.Features.Attributes.Queries.Responses;

public record GetAttributeQueryResponse(int AttributeId, string EnglishName, string ArabicName,IEnumerable<GetAttributeValuesQueryResponse> values);

public record GetAttributeValuesQueryResponse(int AttributeValueId, string EnglishName, string ArabicName);