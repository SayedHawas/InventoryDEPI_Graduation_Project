using smERP.Domain.Entities.Product;
using smERP.SharedKernel.Localizations.Extensions;
using smERP.SharedKernel.Localizations.Resources;
using smERP.SharedKernel.Responses;
using System.Net;

namespace smERP.Domain.ValueObjects;

public class BilingualName : ValueObject
{
    public string English { get; private set; } = null!;
    public string Arabic { get; private set; } = null!;

    private BilingualName(string englishName, string arabicName)
    {
        English = englishName;
        Arabic = arabicName;
    }

    private BilingualName() { }

    public static IResult<BilingualName> Create(string englishName, string arabicName)
    {
        if (string.IsNullOrWhiteSpace(englishName))
            return new Result<BilingualName>()
                .WithError(SharedResourcesKeys.Required_FieldName.Localize(SharedResourcesKeys.Name.Localize()))
                .WithStatusCode(HttpStatusCode.BadRequest);

        if (string.IsNullOrWhiteSpace(arabicName))
            return new Result<BilingualName>()
                .WithError(SharedResourcesKeys.Required_FieldName.Localize(SharedResourcesKeys.Name.Localize()))
                .WithStatusCode(HttpStatusCode.BadRequest);

        if (!englishName.Any(c => char.IsLetter(c) && c <= 127))
            return new Result<BilingualName>()
                .WithError(SharedResourcesKeys.NameAtleastOneLetter.Localize(SharedResourcesKeys.NameEn.Localize()))
                .WithStatusCode(HttpStatusCode.BadRequest);

        if (!arabicName.Any(c => c >= 0x0600 && c <= 0x06FF)) 
            return new Result<BilingualName>()
                .WithError(SharedResourcesKeys.NameAtleastOneLetter.Localize(SharedResourcesKeys.NameAr.Localize()))
                .WithStatusCode(HttpStatusCode.BadRequest);

        return new Result<BilingualName>(new BilingualName(englishName, arabicName));
    }

    public IResult<BilingualName> UpdateEnglish(string newEnglishName)
    {
        if (string.IsNullOrWhiteSpace(newEnglishName))
            return new Result<BilingualName>()
                .WithError(SharedResourcesKeys.Required_FieldName.Localize(SharedResourcesKeys.Name.Localize()))
                .WithStatusCode(HttpStatusCode.BadRequest);

        if (!newEnglishName.Any(c => char.IsLetter(c) && c <= 127))
            return new Result<BilingualName>()
                .WithError(SharedResourcesKeys.NameAtleastOneLetter.Localize(SharedResourcesKeys.NameEn.Localize()))
                .WithStatusCode(HttpStatusCode.BadRequest);

        English = newEnglishName;
        return new Result<BilingualName>(this);
    }

    public IResult<BilingualName> UpdateArabic(string newArabicName)
    {
        if (string.IsNullOrWhiteSpace(newArabicName))
            return new Result<BilingualName>()
                .WithError(SharedResourcesKeys.Required_FieldName.Localize(SharedResourcesKeys.Name.Localize()))
                .WithStatusCode(HttpStatusCode.BadRequest);

        if (!newArabicName.Any(c => c >= 0x0600 && c <= 0x06FF))
            return new Result<BilingualName>()
                .WithError(SharedResourcesKeys.NameAtleastOneLetter.Localize(SharedResourcesKeys.NameAr.Localize()))
                .WithStatusCode(HttpStatusCode.BadRequest);

        Arabic = newArabicName;
        return new Result<BilingualName>(this);
    }


    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return English;
        yield return Arabic;
    }
}