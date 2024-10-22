using FluentValidation;
using smERP.Application.Features.StorageLocations.Commands.Models;
using smERP.SharedKernel.Localizations.Extensions;
using smERP.SharedKernel.Localizations.Resources;

namespace smERP.Application.Features.StorageLocations.Commands.Validators;

public class AddStorageLocationCommandValidator : AbstractValidator<AddStorageLocationCommandModel>
{
    public AddStorageLocationCommandValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty()
            .WithMessage(SharedResourcesKeys.Required_FieldName.Localize(SharedResourcesKeys.Name.Localize()));

        RuleFor(command => command.BranchId)
            .GreaterThan(0)
            .WithMessage(SharedResourcesKeys.Required_FieldName.Localize(SharedResourcesKeys.Branch.Localize()));
    }
}
