using FluentValidation;
using smERP.Application.Features.ProcurementTransactions.Commands.Models;
using smERP.SharedKernel.Localizations.Extensions;
using smERP.SharedKernel.Localizations.Resources;

namespace smERP.Application.Features.ProcurementTransactions.Commands.Validators;

public class EditProcurementTransactionCommandValidator : AbstractValidator<EditProcurementTransactionCommandModelOld>
{
    public EditProcurementTransactionCommandValidator()
    {
        RuleFor(command => command.SupplierId)
            .GreaterThan(0)
            .WithMessage(SharedResourcesKeys.Required_FieldName.Localize(SharedResourcesKeys.Supplier.Localize()));

        RuleForEach(x => x.Products).ChildRules(product =>
        {
            product.RuleFor(p => p.ProductInstanceId)
                .GreaterThan(0)
                .WithMessage(SharedResourcesKeys.Required_FieldName.Localize(SharedResourcesKeys.Product.Localize()));

            product.RuleFor(p => p.Quantity)
                .GreaterThan(0)
                .WithMessage(SharedResourcesKeys.___MustBeAPositiveNumber.Localize(SharedResourcesKeys.Quantity.Localize()));

            product.RuleForEach(p => p.Units)
                .ChildRules(item =>
                {
                    item.RuleFor(i => i.SerialNumber)
                    .NotEmpty()
                    .WithMessage(SharedResourcesKeys.SomeItemsIn___ListAreNotCorrect.Localize(SharedResourcesKeys.Product.Localize()));
                })
                .When(p => p.Units != null);
        });

        RuleForEach(command => command.Payments)
            .Must(payment => payment == null || payment.PayedAmount > 0)
            .WithMessage(SharedResourcesKeys.___MustBeAPositiveNumber.Localize(SharedResourcesKeys.PayedAmount.Localize()));

    }
}

public class UpdatePurchaseTransactionCommandValidator : AbstractValidator<EditProcurementTransactionCommandModel>
{
    public UpdatePurchaseTransactionCommandValidator()
    {
        RuleFor(x => x.TransactionId)
            .GreaterThan(0).WithMessage("Transaction ID must be a positive integer.");

        When(x => x.SupplierId.HasValue, () =>
        {
            RuleFor(x => x.SupplierId.Value)
                .GreaterThan(0).WithMessage("Supplier ID must be a positive integer.");
        });

        When(x => x.ItemUpdates != null, () =>
        {
            RuleForEach(x => x.ItemUpdates)
                .SetValidator(new ItemUpdateValidator());
        });

        When(x => x.ItemsToRemove != null, () =>
        {
            RuleForEach(x => x.ItemsToRemove)
                .NotEmpty().WithMessage("Item ID to remove must not be empty.");
        });

        When(x => x.NewItems != null, () =>
        {
            RuleForEach(x => x.NewItems)
                .SetValidator(new NewItemValidator());
        });

        // New rule to check for duplicate serial numbers across the entire request
        RuleFor(x => x)
            .Must(cmd => !HasDuplicateSerialNumbers(cmd))
            .WithMessage("Duplicate serial numbers found across the request.");
    }

    private bool HasDuplicateSerialNumbers(EditProcurementTransactionCommandModel command)
    {
        var allSerialNumbers = new HashSet<string>();

        // Collect serial numbers from new items
        if (command.NewItems != null)
        {
            foreach (var item in command.NewItems)
            {
                if (item.Units != null)
                {
                    foreach (var serialNumber in item.Units)
                    {
                        if (!allSerialNumbers.Add(serialNumber.SerialNumber))
                        {
                            return true; // Duplicate found
                        }
                    }
                }
            }
        }

        // Collect serial numbers from item updates
        if (command.ItemUpdates != null)
        {
            foreach (var update in command.ItemUpdates)
            {
                if (update.UnitUpdates != null)
                {
                    if (update.UnitUpdates.ToAdd != null)
                    {
                        foreach (var unit in update.UnitUpdates.ToAdd)
                        {
                            if (!allSerialNumbers.Add(unit.SerialNumber))
                            {
                                return true; // Duplicate found
                            }
                        }
                    }
                }
            }
        }

        return false; // No duplicates found
    }

    public class ItemUpdateValidator : AbstractValidator<ItemUpdate>
    {
        public ItemUpdateValidator()
        {
            RuleFor(x => x.ProductInstanceId)
                .GreaterThan(0).WithMessage("Product Instance ID must be a positive integer.");

            When(x => x.UnitPrice.HasValue, () =>
            {
                RuleFor(x => x.UnitPrice.Value)
                    .GreaterThan(0).WithMessage("Unit price must be greater than zero.");
            });

            When(x => x.Quantity.HasValue, () =>
            {
                RuleFor(x => x.Quantity.Value)
                    .GreaterThan(0).WithMessage("Quantity must be greater than zero.");
            });

            When(x => x.UnitUpdates != null, () =>
            {
                RuleFor(x => x.UnitUpdates)
                    .SetValidator(new SerialNumberUpdateValidator());
            });
        }
    }

    public class SerialNumberUpdateValidator : AbstractValidator<UnitUpdates>
    {
        public SerialNumberUpdateValidator()
        {
            When(x => x.ToAdd != null, () =>
            {
                RuleForEach(x => x.ToAdd)
                    .NotEmpty().WithMessage("Serial numbers to add must not be empty.");
            });

            When(x => x.ToRemove != null, () =>
            {
                RuleForEach(x => x.ToRemove)
                    .NotEmpty().WithMessage("Serial numbers to remove must not be empty.");
            });

            RuleFor(x => x)
                .Must(x => (x.ToAdd != null && x.ToAdd.Count > 0) || (x.ToRemove != null && x.ToRemove.Count > 0))
                .WithMessage("At least one of ToAdd or ToRemove must contain serial numbers.");
        }
    }

    public class NewItemValidator : AbstractValidator<NewItem>
    {
        public NewItemValidator()
        {
            RuleFor(x => x.UnitPrice)
                .GreaterThan(0).WithMessage("Unit price must be greater than zero.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than zero.");

            When(x => x.Units != null, () =>
            {
                RuleFor(x => x.Units.Count)
                    .Equal(x => x.Quantity)
                    .WithMessage("The number of serial numbers must match the quantity.");

                RuleForEach(x => x.Units)
                    .NotEmpty().WithMessage("Serial numbers must not be empty.");
            });
        }
    }
}