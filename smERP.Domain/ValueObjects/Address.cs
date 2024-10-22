using smERP.SharedKernel.Localizations.Extensions;
using smERP.SharedKernel.Localizations.Resources;
using smERP.SharedKernel.Responses;
using System.Net;

namespace smERP.Domain.ValueObjects;

public class Address : ValueObject
{
    public int Id { get; private set; }
    public string Street { get; private set; } = null!;
    public string City { get; private set; } = null!;
    public string State { get; private set; } = null!;
    public string Country { get; private set; } = null!;
    public string PostalCode { get; private set; } = null!;
    public string? Comment { get; private set; }

    private Address(string street, string city, string state, string country, string postalCode, string? comment)
    {
        Street = street;
        City = city;
        State = state;
        Country = country;
        PostalCode = postalCode;
        Comment = comment;
    }

    private Address() { }

    public static IResult<Address> Create(string street, string city, string state, string country, string postalCode, string? comment)
    {
        if (string.IsNullOrWhiteSpace(street))
            return new Result<Address>()
                .WithError(SharedResourcesKeys.Required_FieldName.Localize(SharedResourcesKeys.Street.Localize()))
                .WithStatusCode(HttpStatusCode.BadRequest);

        if (string.IsNullOrWhiteSpace(city))
            return new Result<Address>()
                .WithError(SharedResourcesKeys.Required_FieldName.Localize(SharedResourcesKeys.City.Localize()))
                .WithStatusCode(HttpStatusCode.BadRequest);

        if (string.IsNullOrWhiteSpace(state))
            return new Result<Address>()
                .WithError(SharedResourcesKeys.Required_FieldName.Localize(SharedResourcesKeys.State.Localize()))
                .WithStatusCode(HttpStatusCode.BadRequest);

        if (string.IsNullOrWhiteSpace(country))
            return new Result<Address>()
                .WithError(SharedResourcesKeys.Required_FieldName.Localize(SharedResourcesKeys.Country.Localize()))
                .WithStatusCode(HttpStatusCode.BadRequest);

        if (string.IsNullOrWhiteSpace(postalCode))
            return new Result<Address>()
                .WithError(SharedResourcesKeys.Required_FieldName.Localize(SharedResourcesKeys.PostalCode.Localize()))
                .WithStatusCode(HttpStatusCode.BadRequest);

        return new Result<Address>(new Address(street, city, state, country, postalCode, comment));
    }

    public override string ToString()
    {
        return $"{Street}, {City}, {State}, {Country} {PostalCode}, comment: {Comment}".TrimEnd();
    }

    public string ToSingleLineString()
    {
        return string.Join(", ", new[] { Street, City, State, Country, PostalCode, Comment }.Where(s => !string.IsNullOrWhiteSpace(s)));
    }

    public bool IsDomestic(string homeCountry)
    {
        return string.Equals(Country, homeCountry, StringComparison.OrdinalIgnoreCase);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Street;
        yield return City;
        yield return State;
        yield return Country;
        yield return PostalCode;
    }
}
