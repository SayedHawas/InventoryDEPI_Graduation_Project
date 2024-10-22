using FluentValidation;
using smERP.Application.Features.Companies.Commands.Models;
using smERP.SharedKernel.Localizations.Extensions;
using smERP.SharedKernel.Localizations.Resources;

namespace smERP.Application.Features.Companies.Commands.Validators;

public class AddCompanyCommandValidator : AbstractValidator<AddCompanyCommandModel>
{
    public AddCompanyCommandValidator()
    {
        RuleFor(c => c.CompanyName).NotEmpty().WithMessage(c =>
        {
            var fieldName = SharedResourcesKeys.CompanyName.Localize();
            var errorMessage = SharedResourcesKeys.Required_FieldName.Localize(fieldName);
            return errorMessage;
        });
    }
}
