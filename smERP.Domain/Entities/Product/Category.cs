using smERP.Domain.ValueObjects;
using smERP.SharedKernel.Localizations.Extensions;
using smERP.SharedKernel.Localizations.Resources;
using smERP.SharedKernel.Responses;
using System;
using System.Collections.Generic;
using System.Net;

namespace smERP.Domain.Entities.Product;

public class Category : Entity, IAggregateRoot, ICommonEntitiesAttributes
{
    public int? ParentCategoryId { get; private set; }
    public BilingualName Name { get; private set; } = null!;
    public bool IsLeaf { get; private set; }
    public int ProductCount { get; private set; }
    public virtual ICollection<Category> InverseParentCategory { get; private set; } = new List<Category>();
    public virtual Category? ParentCategory { get; private set; }
    public virtual IReadOnlyCollection<Product> Products { get; } = new List<Product>();
    private bool IsHidden { get; set; }
    public DateTime CreatedAt { get; init; }
    public int CreatedBy { get; init; }
    public DateTime UpdatedAt { get; init; }
    public int UpdatedBy { get; init; }

    private Category(BilingualName name, int? parentCategoryId = null)
    {
        var currentDateTime = DateTime.UtcNow;
        Name = name;
        ParentCategoryId = parentCategoryId;
        CreatedAt = currentDateTime;
        UpdatedAt = currentDateTime;
        IsHidden = false;
    }

    private Category() { }

    public static IResult<Category> Create(string englishName, string arabicName, int? parentCategoryId = null)
    {
        var nameResult = BilingualName.Create(englishName, arabicName);
        if (nameResult.IsFailed)
            return nameResult.ChangeType(new Category());

        return new Result<Category>(new Category(nameResult.Value, parentCategoryId))
            .WithStatusCode(HttpStatusCode.Created)
            .WithMessage(SharedResourcesKeys.Created.Localize());
    }

    public IResult<Category> UpdateName(string englishName, string arabicName)
    {
        var nameResult = BilingualName.Create(englishName, arabicName);
        if (nameResult.IsFailed)
            return nameResult.ChangeType(new Category());

        Name = nameResult.Value;
        return new Result<Category>(this)
            .WithStatusCode(HttpStatusCode.OK)
            .WithMessage(SharedResourcesKeys.UpdatedSuccess.Localize());
    }

    public IResult<Category> UpdateEnglishName(string englishName)
    {
        var nameResult = Name.UpdateEnglish(englishName);
        if (nameResult.IsFailed)
            return nameResult.ChangeType(new Category());

        Name = nameResult.Value;
        return new Result<Category>(this)
            .WithStatusCode(HttpStatusCode.OK)
            .WithMessage(SharedResourcesKeys.UpdatedSuccess.Localize());
    }

    public IResult<Category> UpdateArabicName(string arabicName)
    {
        var nameResult = Name.UpdateArabic(arabicName);
        if (nameResult.IsFailed)
            return nameResult.ChangeType(new Category());

        Name = nameResult.Value;
        return new Result<Category>(this)
            .WithStatusCode(HttpStatusCode.OK)
            .WithMessage(SharedResourcesKeys.UpdatedSuccess.Localize());
    }

    public IResult<Category> UpdateParentCategory(int? parentCategoryId)
    {
        if (parentCategoryId == Id)
        {
            var result = new Result<Category>()
                .WithError(SharedResourcesKeys.BadRequest.Localize())
                .WithMessage(SharedResourcesKeys.BadRequest.Localize())
                .WithStatusCode(HttpStatusCode.BadRequest);
            return result;
        }

        ParentCategoryId = parentCategoryId;
        return new Result<Category>(this)
            .WithStatusCode(HttpStatusCode.OK)
            .WithMessage(SharedResourcesKeys.UpdatedSuccess.Localize());
    }

    public void AddSubcategory(Category subcategory)
    {
        InverseParentCategory.Add(subcategory);
    }

    public void RemoveSubcategory(Category subcategory)
    {
        InverseParentCategory.Remove(subcategory);
    }

    public void Hide()
    {
        IsHidden = true;
    }

    public void Show()
    {
        IsHidden = false;
    }
}
