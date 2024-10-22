using FluentValidation;
using smERP.Application.Features.Attributes.Commands.Models;
using smERP.SharedKernel.Localizations.Extensions;
using smERP.SharedKernel.Localizations.Resources;

namespace smERP.Application.Features.Attributes.Commands.Validators;

public class AddAttributeValueCommandValidator : AbstractValidator<AddAttributeValueCommandModel>
{
    public AddAttributeValueCommandValidator()
    {
        RuleFor(c => c.AttributeId).NotEmpty().GreaterThan(0).WithMessage(c =>
        {
            var fieldName = SharedResourcesKeys.Attribute.Localize();
            var errorMessage = SharedResourcesKeys.Required_FieldName.Localize(fieldName);
            return errorMessage;
        });

        RuleFor(c => c.ArabicName).NotEmpty().WithMessage(c =>
        {
            var fieldName = SharedResourcesKeys.NameAr.Localize();
            var errorMessage = SharedResourcesKeys.Required_FieldName.Localize(fieldName);
            return errorMessage;
        });

        RuleFor(c => c.EnglishName).NotEmpty().WithMessage(c =>
        {
            var fieldName = SharedResourcesKeys.NameEn.Localize();
            var errorMessage = SharedResourcesKeys.Required_FieldName.Localize(fieldName);
            return errorMessage;
        });
    }
}
