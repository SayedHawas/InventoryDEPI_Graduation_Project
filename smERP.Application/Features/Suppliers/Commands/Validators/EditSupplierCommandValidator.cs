using FluentValidation;
using smERP.Application.Features.Suppliers.Commands.Models;
using smERP.SharedKernel.Localizations.Extensions;
using smERP.SharedKernel.Localizations.Resources;

namespace smERP.Application.Features.Suppliers.Commands.Validators;

public class EditSupplierCommandValidator : AbstractValidator<EditSupplierCommandModel>
{
    public EditSupplierCommandValidator()
    {
        RuleFor(c => c.SupplierId)
            .GreaterThan(0)
            .WithMessage(SharedResourcesKeys.Required_FieldName.Localize(SharedResourcesKeys.Supplier.Localize()));

        When(c => c.Addresses != null && c.Addresses.Any(), () =>
        {
            RuleForEach(c => c.Addresses)
                .ChildRules(address =>
                {
                    address.RuleFor(a => a.Street)
                        .NotEmpty()
                        .WithMessage(SharedResourcesKeys.Required_FieldName.Localize(SharedResourcesKeys.Street.Localize()));

                    address.RuleFor(a => a.City)
                        .NotEmpty()
                        .WithMessage(SharedResourcesKeys.Required_FieldName.Localize(SharedResourcesKeys.City.Localize()));

                    address.RuleFor(a => a.State)
                        .NotEmpty()
                        .WithMessage(SharedResourcesKeys.Required_FieldName.Localize(SharedResourcesKeys.State.Localize()));

                    address.RuleFor(a => a.Country)
                        .NotEmpty()
                        .WithMessage(SharedResourcesKeys.Required_FieldName.Localize(SharedResourcesKeys.Country.Localize()));

                    address.RuleFor(a => a.PostalCode)
                        .NotEmpty()
                        .WithMessage(SharedResourcesKeys.Required_FieldName.Localize(SharedResourcesKeys.PostalCode.Localize()));
                });
        });
    }
}
