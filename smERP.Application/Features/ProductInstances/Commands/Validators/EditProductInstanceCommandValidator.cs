using FluentValidation;
using smERP.Application.Features.ProductInstances.Commands.Models;
using smERP.SharedKernel.Localizations.Extensions;
using smERP.SharedKernel.Localizations.Resources;

namespace smERP.Application.Features.ProductInstances.Commands.Validators;

public class EditProductInstanceCommandValidator : AbstractValidator<EditProductInstanceCommandModel>
{
    private readonly long _maxFileSizeBytes = 10 * 1024 * 1024; // 10 MB
    public EditProductInstanceCommandValidator()
    {
        RuleFor(command => command.ProductId)
            .GreaterThan(0)
            .WithMessage(SharedResourcesKeys.Required_FieldName.Localize(SharedResourcesKeys.Product.Localize()));

        RuleFor(command => command.ProductInstanceId)
            .GreaterThan(0)
            .WithMessage(SharedResourcesKeys.Required_FieldName.Localize(SharedResourcesKeys.Product.Localize()));

        //RuleFor(command => command.BuyingPrice)
        //    .Must(MustBePositiveIfNotNull)
        //    .WithMessage(SharedResourcesKeys.___MustBeAPositiveNumber.Localize(SharedResourcesKeys.BuyingPrice.Localize()));

        RuleFor(command => command.SellingPrice)
            .Must(MustBePositiveIfNotNull)
            .WithMessage(SharedResourcesKeys.___MustBeAPositiveNumber.Localize(SharedResourcesKeys.SellingPrice.Localize()));

        //RuleFor(command => command.QuantityInStock)
        //    .Must(MustBePositiveIfNotNull)
        //    .WithMessage(SharedResourcesKeys.___MustBeAPositiveNumber.Localize(SharedResourcesKeys.Quantity.Localize()));

        RuleFor(command => command.Attributes)
            .Must(MustHaveUniqueAttributeIdsIfNotNull)
            .WithMessage(SharedResourcesKeys.___ListCannotContainDuplicates.Localize(SharedResourcesKeys.AttributeList.Localize()));

        RuleForEach(x => x.ImagesBase64)
            .Must(BeValidBase64).WithMessage(SharedResourcesKeys.Invalid___.Localize(SharedResourcesKeys.Image.Localize()))
            .Must(BeValidFileSize).WithMessage(SharedResourcesKeys.FileSizeExceedsTheMaximumAllowedSizeOf___MB.Localize("10"))
            .When(x => x.ImagesBase64 != null && x.ImagesBase64.Count > 0);
    }

    private bool MustHaveUniqueAttributeIdsIfNotNull(List<ProductInstanceAttributeValue>? attributeValues)
    {
        if (attributeValues == null)
            return true;

        return attributeValues.DistinctBy(x => x.AttributeId).Count() == attributeValues.Count;
    }

    private bool MustBePositiveIfNotNull(decimal? number)
    {
        if (number == null)
            return true;

        if (number > 0)
            return true;

        return false;
    }

    private bool MustBePositiveIfNotNull(int? number)
    {
        if (number == null)
            return true;

        if (number > 0)
            return true;

        return false;
    }


    private bool BeValidBase64(string base64String)
    {
        if (string.IsNullOrWhiteSpace(base64String)) return false;
        try
        {
            Convert.FromBase64String(base64String.Split(",")[1]);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private bool BeValidFileSize(string base64String)
    {
        if (string.IsNullOrWhiteSpace(base64String)) return false;
        long fileSizeBytes = (base64String.Length * 3) / 4;
        return fileSizeBytes <= _maxFileSizeBytes;
    }
}