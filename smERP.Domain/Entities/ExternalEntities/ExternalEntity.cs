using smERP.Domain.ValueObjects;
using smERP.SharedKernel.Localizations.Extensions;
using smERP.SharedKernel.Localizations.Resources;
using smERP.SharedKernel.Responses;
using System.Net;

namespace smERP.Domain.Entities.ExternalEntities;

public class ExternalEntity : Entity
{
    public BilingualName Name { get; private set; } = null!;
    public virtual ICollection<Address> Addresses { get; private set; } = new List<Address>();

    protected ExternalEntity(BilingualName name, List<Address> addresses)
    {
        Name = name;
        Addresses = addresses;
    }

    protected ExternalEntity() { }

    protected static IResult<(BilingualName Name, List<Address> Addresses)> CreateBaseDetails(string englishName, string arabicName, IEnumerable<(string street, string city, string state, string country, string postalCode, string? comment)> addresses)
    {
        var nameResult = BilingualName.Create(englishName, arabicName);
        if (nameResult.IsFailed)
            return new Result<(BilingualName, List<Address>)>()
                .WithErrors(nameResult.Errors)
                .WithStatusCode(HttpStatusCode.BadRequest);

        var addressList = new List<Address>();
        foreach (var (street, city, state, country, postalCode, comment) in addresses)
        {
            var addressResult = Address.Create(street, city, state, country, postalCode, comment);
            if (addressResult.IsFailed)
                return new Result<(BilingualName, List<Address>)>()
                    .WithErrors(addressResult.Errors)
                    .WithStatusCode(HttpStatusCode.BadRequest);

            addressList.Add(addressResult.Value);
        }

        return new Result<(BilingualName, List<Address>)>((nameResult.Value, addressList));
    }

    public IResult<Address> AddAddress(string street, string city, string state, string country, string postalCode, string? comment)
    {
        var addressCreateResult = Address.Create(street, city, state, country, postalCode, comment);
        if (addressCreateResult.IsFailed)
            return addressCreateResult;

        Addresses.Add(addressCreateResult.Value);
        return addressCreateResult;
    }

    public IResult<List<Address>> UpdateAddresses(IEnumerable<(string street, string city, string state, string country, string postalCode, string? comment)> addresses)
    {
        if (addresses == null || addresses.Count() < 1)
            return new Result<List<Address>>()
                .WithError(SharedResourcesKeys.___ListMustContainAtleastOneItem.Localize(SharedResourcesKeys.Address.Localize()))
                .WithStatusCode(HttpStatusCode.BadRequest);

        Addresses.Clear();

        foreach (var (street, city, state, country, postalCode, comment) in addresses)
        {
            var addressResult = Address.Create(street, city, state, country, postalCode, comment);
            if (addressResult.IsFailed)
                return new Result<List<Address>>()
                    .WithErrors(addressResult.Errors)
                    .WithStatusCode(HttpStatusCode.BadRequest);

            Addresses.Add(addressResult.Value);
        }
        return new Result<List<Address>>(Addresses.ToList());
    }

    public IResult<Address> UpdateAddress(Address existingAddress, string? street = null, string? city = null, string? state = null, string? country = null, string? postalCode = null, string? comment = null)
    {
        if (!Addresses.Contains(existingAddress))
        {
            return new Result<Address>()
                .WithError(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.Address.Localize()))
                .WithStatusCode(HttpStatusCode.BadRequest);
        }

        var updatedAddressResult = Address.Create(
            street ?? existingAddress.Street,
            city ?? existingAddress.City,
            state ?? existingAddress.State,
            country ?? existingAddress.Country,
            postalCode ?? existingAddress.PostalCode,
            comment ?? existingAddress.Comment
        );

        if (updatedAddressResult.IsFailed)
            return updatedAddressResult;

        Addresses.Remove(existingAddress);
        Addresses.Add(updatedAddressResult.Value);

        return updatedAddressResult;
    }

    public IResult<Address> RemoveAddress(Address address)
    {
        if (!Addresses.Remove(address))
        {
            return new Result<Address>()
                .WithError(SharedResourcesKeys.DoesNotExist.Localize(SharedResourcesKeys.Address.Localize()))
                .WithStatusCode(HttpStatusCode.BadRequest);
        }

        return new Result<Address>();
    }
}

