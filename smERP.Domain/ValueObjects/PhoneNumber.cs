
using smERP.SharedKernel.Localizations.Extensions;
using smERP.SharedKernel.Localizations.Resources;
using smERP.SharedKernel.Responses;
using System.Net;
using System.Text.RegularExpressions;

namespace smERP.Domain.ValueObjects;

public class PhoneNumber : ValueObject
{
    public string CountryCode { get; } = null!;
    public string Number { get; } = null!;
    public string? Comment { get; }

    private PhoneNumber() { }

    public PhoneNumber(string countryCode, string number, string? comment)
    {
        CountryCode = countryCode;
        Number = number;
        Comment = comment;
    }

    public static IResult<PhoneNumber> Create(string phoneNumber, string? comment)
    {
        // Remove any non-digit characters
        var digitsOnly = Regex.Replace(phoneNumber, @"\D", "");

        if (string.IsNullOrWhiteSpace(digitsOnly))
            return new Result<PhoneNumber>()
                .WithError(SharedResourcesKeys.Required_FieldName.Localize(SharedResourcesKeys.PhoneNumber))
                .WithStatusCode(HttpStatusCode.BadRequest);

        if (digitsOnly.Length < 10 || digitsOnly.Length > 15)
            return new Result<PhoneNumber>()
                .WithError(SharedResourcesKeys.___MustBeBetween_And_.Localize(SharedResourcesKeys.PhoneNumber.Localize(),"10","15"))
                .WithStatusCode(HttpStatusCode.BadRequest);

        string countryCode = "";
        string number = digitsOnly;

        // If the number starts with a plus, assume the first 1-3 digits are the country code
        if (phoneNumber.StartsWith('+'))
        {
            int countryCodeLength = Math.Min(3, digitsOnly.Length - 9);
            countryCode = digitsOnly[..countryCodeLength];
            number = digitsOnly[countryCodeLength..];
        }

        return new Result<PhoneNumber>(new PhoneNumber(countryCode, number, comment));
    }

    public string ToFormattedString()
    {
        if (!string.IsNullOrEmpty(CountryCode))
            return $"+{CountryCode} {FormatNationalNumber()}";
        return FormatNationalNumber();
    }

    private string FormatNationalNumber()
    {
        // This is a simple formatting. You might want to adjust based on specific country formats.
        if (Number.Length == 10)
            return $"({Number.Substring(0, 3)}) {Number.Substring(3, 3)}-{Number.Substring(6)}";
        return Number;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return CountryCode;
        yield return Number;
    }

    public override string ToString()
    {
        return ToFormattedString();
    }
}
