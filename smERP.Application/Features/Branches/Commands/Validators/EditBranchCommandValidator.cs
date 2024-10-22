using FluentValidation;
using smERP.Application.Features.Branches.Commands.Models;
using smERP.SharedKernel.Localizations.Extensions;
using smERP.SharedKernel.Localizations.Resources;

namespace smERP.Application.Features.Branches.Commands.Validators;

public class EditBranchCommandValidator : AbstractValidator<EditBranchCommandModel>
{
    public EditBranchCommandValidator()
    {
        RuleFor(c => c.BranchId).NotEmpty().GreaterThan(0).WithMessage(c =>
        {
            var fieldName = SharedResourcesKeys.Branch.Localize();
            var errorMessage = SharedResourcesKeys.Required_FieldName.Localize(fieldName);
            return errorMessage;
        });
    }
}
