using FluentValidation;
using smERP.Application.Features.StorageLocations.Commands.Models;
using smERP.SharedKernel.Localizations.Extensions;
using smERP.SharedKernel.Localizations.Resources;

namespace smERP.Application.Features.StorageLocations.Commands.Validators;

public class AddProductInstanceToStorageLocationValidator :AbstractValidator<AddProductInstanceToStorageLocationModel>
{
    public AddProductInstanceToStorageLocationValidator()
    {
        RuleFor(command => command.StorageLocationId)
            .GreaterThan(0)
            .WithMessage(SharedResourcesKeys.Required_FieldName.Localize(SharedResourcesKeys.StorageLocation.Localize()));

        RuleForEach(x => x.Products).ChildRules(product =>
        {
            product.RuleFor(p => p.ProductInstanceId)
                .GreaterThan(0)
                .WithMessage(SharedResourcesKeys.Required_FieldName.Localize(SharedResourcesKeys.Product.Localize()));

            product.RuleFor(p => p.Quantity)
                .GreaterThan(0)
                .WithMessage(SharedResourcesKeys.___MustBeAPositiveNumber.Localize(SharedResourcesKeys.Quantity.Localize()));

            product.RuleFor(p => p.Units.Count)
                .Equal(p => p.Quantity)
                .When(p => p.Units != null)
                .WithMessage(SharedResourcesKeys.SomeItemsIn___ListAreNotCorrect.Localize(SharedResourcesKeys.Product.Localize()));

            product.RuleForEach(p => p.Units)
            .ChildRules(item =>
            {
                item.RuleFor(i => i.SerialNumber)
                .NotEmpty()
                .WithMessage(SharedResourcesKeys.SomeItemsIn___ListAreNotCorrect.Localize(SharedResourcesKeys.Product.Localize()));

                item.RuleFor(i => i.ExpirationDate)
                    .GreaterThan(DateOnly.FromDateTime(DateTime.UtcNow))
                    .When(i => i.ExpirationDate.HasValue)
                    .WithMessage(SharedResourcesKeys.ExpirationDateMustBeInTheFuture.Localize());
            })
            .When(p => p.Units != null);
        });
    }
}
