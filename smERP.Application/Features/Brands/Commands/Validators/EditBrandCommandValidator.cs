using FluentValidation;
using smERP.Application.Features.Brands.Commands.Models;
using smERP.SharedKernel.Localizations.Extensions;
using smERP.SharedKernel.Localizations.Resources;

namespace smERP.Application.Features.Brands.Commands.Validators;

public class EditBrandCommandValidator : AbstractValidator<EditBrandCommandModel>
{
    public EditBrandCommandValidator()
    {
        RuleFor(c => c.BrandId).NotEmpty().GreaterThan(0).WithMessage(c =>
        {
            var fieldName = SharedResourcesKeys.Brand.Localize();
            var errorMessage = SharedResourcesKeys.Required_FieldName.Localize(fieldName);
            return errorMessage;
        });
    }
}
