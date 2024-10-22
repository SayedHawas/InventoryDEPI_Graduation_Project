using FluentValidation;
using smERP.Application.Features.StorageLocations.Commands.Models;
using smERP.SharedKernel.Localizations.Extensions;
using smERP.SharedKernel.Localizations.Resources;

namespace smERP.Application.Features.StorageLocations.Commands.Validators;

public class EditStorageLocationCommandValidator : AbstractValidator<EditStorageLocationCommandModel>
{
    public EditStorageLocationCommandValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty()
            .WithMessage(SharedResourcesKeys.Required_FieldName.Localize(SharedResourcesKeys.Name.Localize()));

        RuleFor(command => command.BranchId)
            .GreaterThan(0)
            .WithMessage(SharedResourcesKeys.Required_FieldName.Localize(SharedResourcesKeys.Branch.Localize()));

        RuleFor(command => command.StorageLocationId)
            .GreaterThan(0)
            .WithMessage(SharedResourcesKeys.Required_FieldName.Localize(SharedResourcesKeys.StorageLocation.Localize()));
    }
}
