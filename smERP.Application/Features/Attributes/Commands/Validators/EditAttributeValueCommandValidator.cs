using FluentValidation;
using smERP.Application.Features.Attributes.Commands.Models;
using smERP.SharedKernel.Localizations.Extensions;
using smERP.SharedKernel.Localizations.Resources;

namespace smERP.Application.Features.Attributes.Commands.Validators;

public class EditAttributeValueCommandValidator : AbstractValidator<EditAttributeValueCommandModel>
{
    public EditAttributeValueCommandValidator()
    {
        RuleFor(c => c.AttributeValueId).NotEmpty().GreaterThan(0).WithMessage(c =>
        {
            var fieldName = SharedResourcesKeys.AttributeValue.Localize();
            var errorMessage = SharedResourcesKeys.Required_FieldName.Localize(fieldName);
            return errorMessage;
        });

        RuleFor(c => c.AttributeId).NotEmpty().GreaterThan(0).WithMessage(c =>
        {
            var fieldName = SharedResourcesKeys.Attribute.Localize();
            var errorMessage = SharedResourcesKeys.Required_FieldName.Localize(fieldName);
            return errorMessage;
        });
    }
}
