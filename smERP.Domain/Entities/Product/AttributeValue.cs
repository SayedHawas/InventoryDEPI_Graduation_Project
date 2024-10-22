using smERP.Domain.ValueObjects;
using smERP.SharedKernel.Localizations.Extensions;
using smERP.SharedKernel.Localizations.Resources;
using smERP.SharedKernel.Responses;
using System.Net;

namespace smERP.Domain.Entities.Product;

public class AttributeValue : Entity
{
    public int AttributeId { get; private set; }
    public BilingualName Value { get; private set; } = null!;
    public virtual Attribute Attribute { get; private set; } = null!;
    //public virtual IReadOnlyCollection<ProductInstanceAttribute> ProductInstanceAttributes { get; } = new List<ProductInstanceAttribute>();
    //public virtual IReadOnlyCollection<ProductInstanceAttributeValue> ProductInstanceAttributeValues { get; } = new List<ProductInstanceAttributeValue>();

    private AttributeValue(int attributeId, BilingualName value)
    {
        AttributeId = attributeId;
        Value = value;
    }

    internal AttributeValue() { }

    internal static AttributeValue Create(int attributeId, BilingualName value) =>
        new(attributeId, value);

    public static IResult<AttributeValue> Create(int attributeId, string englishValue, string arabicValue)
    {
        if (string.IsNullOrWhiteSpace(englishValue) || string.IsNullOrWhiteSpace(arabicValue))
        {
            var localizedValueName = SharedResourcesKeys.AttributeValue.Localize();
            var result = new Result<AttributeValue>()
                .WithError(SharedResourcesKeys.Required_FieldName.Localize(localizedValueName))
                .WithMessage(SharedResourcesKeys.BadRequest.Localize())
                .WithStatusCode(HttpStatusCode.BadRequest);
            return result;
        }

        var valueResult = BilingualName.Create(englishValue, arabicValue);
        if (valueResult.IsFailed)
            return valueResult.ChangeType(new AttributeValue());

        return new Result<AttributeValue>(new AttributeValue(attributeId, valueResult.Value))
            .WithStatusCode(HttpStatusCode.Created)
            .WithMessage(SharedResourcesKeys.Created.Localize());
    }

    public IResult<AttributeValue> UpdateValue(string englishValue, string arabicValue)
    {
        if (string.IsNullOrWhiteSpace(englishValue) || string.IsNullOrWhiteSpace(arabicValue))
        {
            var localizedValueName = SharedResourcesKeys.AttributeValue.Localize();
            var result = new Result<AttributeValue>()
                .WithError(SharedResourcesKeys.Required_FieldName.Localize(localizedValueName))
                .WithMessage(SharedResourcesKeys.BadRequest.Localize())
                .WithStatusCode(HttpStatusCode.BadRequest);
            return result;
        }

        var valueResult = BilingualName.Create(englishValue, arabicValue);
        if (valueResult.IsFailed)
            return valueResult.ChangeType(new AttributeValue());

        Value = valueResult.Value;
        return new Result<AttributeValue>(this)
            .WithStatusCode(HttpStatusCode.OK)
            .WithMessage(SharedResourcesKeys.UpdatedSuccess.Localize());
    }
}
