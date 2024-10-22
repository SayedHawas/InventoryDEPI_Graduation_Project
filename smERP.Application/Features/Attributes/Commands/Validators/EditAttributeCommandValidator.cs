using FluentValidation;
using smERP.Application.Features.Attributes.Commands.Models;
using smERP.SharedKernel.Localizations.Extensions;
using smERP.SharedKernel.Localizations.Resources;

namespace smERP.Application.Features.Attributes.Commands.Validators;

public class EditAttributeCommandValidator : AbstractValidator<EditAttributeCommandModel>
{
    public EditAttributeCommandValidator()
    {
        RuleFor(c => c.AttributeId)
            .NotEmpty()
            .GreaterThan(0)
            .WithMessage(c =>
            {
                var fieldName = SharedResourcesKeys.Attribute.Localize();
                var errorMessage = SharedResourcesKeys.Required_FieldName.Localize(fieldName);
                return errorMessage;
            });

        RuleFor(c => c.EnglishName)
            .NotEmpty()
            .When(c => c.EnglishName != null)
            .WithMessage(c => SharedResourcesKeys.Required_FieldName.Localize(SharedResourcesKeys.NameEn.Localize()));

        RuleFor(c => c.ArabicName)
            .NotEmpty()
            .When(c => c.ArabicName != null)
            .WithMessage(c => SharedResourcesKeys.Required_FieldName.Localize(SharedResourcesKeys.NameAr.Localize()));

        RuleFor(c => c)
            .Custom((command, context) =>
            {
                var allNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                if (command.ValuesToAdd != null)
                {
                    foreach (var value in command.ValuesToAdd)
                    {
                        if (!string.IsNullOrEmpty(value.EnglishName) && !allNames.Add(value.EnglishName))
                        {
                            context.AddFailure(SharedResourcesKeys.___ListCannotContainDuplicates.Localize(SharedResourcesKeys.NameEn.Localize()));
                        }
                        if (!string.IsNullOrEmpty(value.ArabicName) && !allNames.Add(value.ArabicName))
                        {
                            context.AddFailure(SharedResourcesKeys.___ListCannotContainDuplicates.Localize(SharedResourcesKeys.NameAr.Localize()));
                        }
                    }
                }

                if (command.ValuesToEdit != null)
                {
                    foreach (var value in command.ValuesToEdit)
                    {
                        if (value.EnglishName != null && !string.IsNullOrEmpty(value.EnglishName) && !allNames.Add(value.EnglishName))
                        {
                            context.AddFailure(SharedResourcesKeys.___ListCannotContainDuplicates.Localize(SharedResourcesKeys.NameEn.Localize()));
                        }
                        if (value.ArabicName != null && !string.IsNullOrEmpty(value.ArabicName) && !allNames.Add(value.ArabicName))
                        {
                            context.AddFailure(SharedResourcesKeys.___ListCannotContainDuplicates.Localize(SharedResourcesKeys.NameAr.Localize()));
                        }
                    }
                }
            });

        RuleForEach(c => c.ValuesToAdd)
            .SetValidator(new AddAttributeValueModelValidator());

        RuleForEach(c => c.ValuesToEdit)
            .SetValidator(new EditAttributeValueModelValidator());
    }
}

public class AddAttributeValueModelValidator : AbstractValidator<AddAttributeValueModel>
{
    public AddAttributeValueModelValidator()
    {
        RuleFor(v => v.EnglishName)
            .NotEmpty()
            .WithMessage(c => SharedResourcesKeys.Required_FieldName.Localize(SharedResourcesKeys.NameEn.Localize()));

        RuleFor(v => v.ArabicName)
            .NotEmpty()
            .WithMessage(c => SharedResourcesKeys.Required_FieldName.Localize(SharedResourcesKeys.NameAr.Localize()));

    }
}

public class EditAttributeValueModelValidator : AbstractValidator<EditAttributeValueModel>
{
    public EditAttributeValueModelValidator()
    {
        RuleFor(v => v.EnglishName)
            .NotEmpty()
            .When(v => v.EnglishName != null)
            .WithMessage(c => SharedResourcesKeys.Required_FieldName.Localize(SharedResourcesKeys.NameEn.Localize()));

        RuleFor(v => v.ArabicName)
            .NotEmpty()
            .When(v => v.ArabicName != null)
            .WithMessage(c => SharedResourcesKeys.Required_FieldName.Localize(SharedResourcesKeys.NameAr.Localize()));

        RuleFor(v => v.AttributeValueId)
            .GreaterThan(0)
            .WithMessage(c => SharedResourcesKeys.Required_FieldName.Localize(SharedResourcesKeys.AttributeValue.Localize()));
    }
}