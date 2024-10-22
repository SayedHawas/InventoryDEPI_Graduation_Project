using FluentValidation;
using smERP.Application.Features.Attributes.Commands.Models;
using smERP.SharedKernel.Localizations.Extensions;
using smERP.SharedKernel.Localizations.Resources;

namespace smERP.Application.Features.Attributes.Commands.Validators;

public class AddAttributeCommandValidator : AbstractValidator<AddAttributeCommandModel>
{
    public AddAttributeCommandValidator()
    {
        RuleFor(c => c.ArabicName)
            .NotEmpty()
            .WithMessage(SharedResourcesKeys.Required_FieldName.Localize(SharedResourcesKeys.NameAr.Localize()));

        RuleFor(c => c.EnglishName)
            .NotEmpty()
            .WithMessage(SharedResourcesKeys.Required_FieldName.Localize(SharedResourcesKeys.NameEn.Localize()));

        RuleFor(c => c.Values)
            .NotNull()
            .Must(values => values != null && values.Count > 0)
            .WithMessage(SharedResourcesKeys.___ListMustContainAtleastOneItem.Localize(SharedResourcesKeys.AttributeValue.Localize()));

        RuleFor(c => c.Values)
            .Must(values => values != null && values.All(v => !string.IsNullOrEmpty(v.EnglishName) && !string.IsNullOrEmpty(v.ArabicName)))
            .WithMessage(SharedResourcesKeys.SomeItemsIn___ListAreNotCorrect.Localize(SharedResourcesKeys.AttributeValue.Localize()));

        RuleFor(c => c.Values)
            .Must(values => values != null && values.Select(v => v.EnglishName).Distinct().Count() == values.Count)
            .WithMessage(SharedResourcesKeys.___ListCannotContainDuplicates.Localize(SharedResourcesKeys.AttributeValue.Localize()));

        RuleFor(c => c.Values)
            .Must(values => values != null && values.Select(v => v.ArabicName).Distinct().Count() == values.Count)
            .WithMessage(SharedResourcesKeys.___ListCannotContainDuplicates.Localize(SharedResourcesKeys.AttributeValue.Localize()));
    }
}