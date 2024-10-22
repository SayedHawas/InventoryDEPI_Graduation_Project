using FluentValidation;
using smERP.Application.Features.Categories.Commands.Models;
using smERP.SharedKernel.Localizations.Extensions;
using smERP.SharedKernel.Localizations.Resources;

namespace smERP.Application.Features.Categories.Commands.Validators;

public class EditCategoryCommandValidator : AbstractValidator<EditCategoryCommandModel>
{
    public EditCategoryCommandValidator()
    {
        RuleFor(c => c.CategoryId).NotEmpty().WithMessage(c =>
        {
            var fieldName = SharedResourcesKeys.Category.Localize();
            var errorMessage = SharedResourcesKeys.Required_FieldName.Localize(fieldName);
            return errorMessage;
        });
    }
}
