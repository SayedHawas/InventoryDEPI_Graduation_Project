using smERP.Domain.ValueObjects;
using smERP.SharedKernel.Localizations.Extensions;
using smERP.SharedKernel.Localizations.Resources;
using smERP.SharedKernel.Responses;
using System;
using System.Net;

namespace smERP.Domain.Entities.Product;

public class Attribute : Entity, IAggregateRoot
{
    public BilingualName Name { get; private set; } = null!;
    public virtual ICollection<AttributeValue> AttributeValues { get; } = new List<AttributeValue>();
    private bool IsHidden { get; set; }

    private Attribute(BilingualName name)
    {
        Name = name;
        IsHidden = true;
    }

    private Attribute() { }

    public static IResult<Attribute> Create(string englishName, string arabicName)
    {
        var nameResult = BilingualName.Create(englishName, arabicName);
        if (nameResult.IsFailed)
            return nameResult.ChangeType(new Attribute());

        return new Result<Attribute>(new Attribute(nameResult.Value))
            .WithStatusCode(HttpStatusCode.Created)
            .WithMessage(SharedResourcesKeys.Created.Localize());
    }

    public IResult<Attribute> UpdateName(string englishName, string arabicName)
    {
        var nameResult = BilingualName.Create(englishName, arabicName);
        if (nameResult.IsFailed)
            return nameResult.ChangeType(new Attribute());

        Name = nameResult.Value;
        return new Result<Attribute>(this)
            .WithStatusCode(HttpStatusCode.OK)
            .WithMessage(SharedResourcesKeys.UpdatedSuccess.Localize());
    }

    public IResult<AttributeValue> AddAttributeValue(string englishValue, string arabicValue)
    {
        if (AttributeValues.Any(av => av.Value.English.Equals(englishValue, StringComparison.OrdinalIgnoreCase)))
        {
            return new Result<AttributeValue>()
                .WithError(SharedResourcesKeys.DoesExist.Localize(SharedResourcesKeys.NameEn.Localize()))
                .WithStatusCode(HttpStatusCode.BadRequest);
        }

        if (AttributeValues.Any(av => av.Value.Arabic.Equals(arabicValue, StringComparison.OrdinalIgnoreCase)))
        {
            return new Result<AttributeValue>()
                .WithError(SharedResourcesKeys.DoesExist.Localize(SharedResourcesKeys.NameAr.Localize()))
                .WithStatusCode(HttpStatusCode.BadRequest);
        }

        var valueResult = BilingualName.Create(englishValue, arabicValue);
        if (valueResult.IsFailed)
            return valueResult.ChangeType(new AttributeValue());

        var attributeValue = AttributeValue.Create(Id, valueResult.Value);
        AttributeValues.Add(attributeValue);

        return new Result<AttributeValue>(attributeValue)
            .WithStatusCode(HttpStatusCode.Created)
            .WithMessage(SharedResourcesKeys.Created.Localize());
    }

    public IResult<AttributeValue> UpdateAttributeValue(int AttributeValueId, string? englishValue, string? arabicValue)
    {
        var attributeValueToBeEdited = AttributeValues.FirstOrDefault(x => x.AttributeId == Id && x.Id == AttributeValueId);
        if (attributeValueToBeEdited == null)
            return new Result<AttributeValue>()
                .WithError(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.Attribute.Localize()))
                .WithStatusCode(HttpStatusCode.BadRequest);

        if (!string.IsNullOrWhiteSpace(englishValue))
        {
            if (AttributeValues.Any(av => av.Value.English.Equals(englishValue, StringComparison.OrdinalIgnoreCase) && av.Id != AttributeValueId))
            {
                return new Result<AttributeValue>()
                    .WithError(SharedResourcesKeys.DoesExist.Localize(SharedResourcesKeys.NameEn.Localize()))
                    .WithStatusCode(HttpStatusCode.BadRequest);
            }
            attributeValueToBeEdited.Value.UpdateEnglish(englishValue);
        }

        if (!string.IsNullOrWhiteSpace(arabicValue))
        {
            if (AttributeValues.Any(av => av.Value.Arabic.Equals(arabicValue, StringComparison.OrdinalIgnoreCase) && av.Id != AttributeValueId))
            {
                return new Result<AttributeValue>()
                    .WithError(SharedResourcesKeys.DoesExist.Localize(SharedResourcesKeys.NameAr.Localize()))
                    .WithStatusCode(HttpStatusCode.BadRequest);
            }
            attributeValueToBeEdited.Value.UpdateArabic(arabicValue);
        }

        return new Result<AttributeValue>(attributeValueToBeEdited);
    }

    public IResultBase RemoveAttributeValue(int AttributeValueId)
    {
        var attributeValueToBeRemoved = AttributeValues.FirstOrDefault(x => x.Id == AttributeValueId);
        if(attributeValueToBeRemoved == null)
            return new Result<AttributeValue>()
                .WithError(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.AttributeValue.Localize()))
                .WithStatusCode(HttpStatusCode.BadRequest);

        AttributeValues.Remove(attributeValueToBeRemoved);
        return new Result<AttributeValue>(attributeValueToBeRemoved);
    }

    public void Hide()
    {
        IsHidden = true;
    }

    public void Show()
    {
        IsHidden = false;
    }
}