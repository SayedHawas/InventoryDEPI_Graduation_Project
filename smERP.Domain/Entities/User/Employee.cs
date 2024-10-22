using smERP.Domain.Entities.Organization;
using smERP.Domain.ValueObjects;
using smERP.SharedKernel.Localizations.Extensions;
using smERP.SharedKernel.Localizations.Resources;
using smERP.SharedKernel.Responses;
using System.Net;

namespace smERP.Domain.Entities.User;

public class Employee : IAggregateRoot
{
    public string Id { get; private set; } = null!;
    public int BranchId { get; set; }
    public Address? Address { get; private set; }
    public ICollection<PhoneNumber>? PhoneNumbers { get; private set; }
    public virtual Branch Branch { get; private set; } = null!;

    private Employee(string employeeId, int branchId, Address? address, ICollection<PhoneNumber>? phoneNumbers)
    {
        Id = employeeId;
        BranchId = branchId;
        Address = address;
        PhoneNumbers = phoneNumbers;
    }

    public Employee() { }

    public void AddAddress(string country, string city, string state, string street, string postalCode, string? comment)
    {
        Address = Address.Create(country, city, state, street, postalCode, comment).Value;
    }

    public static IResult<Employee> Create(string employeeId, int branchId, (string street, string city, string state, string country, string postalCode, string? comment)? address, List<(string number, string? comment)>? phoneNumbers)
    {
        if (branchId < 0)
            return new Result<Employee>()
                .WithError(SharedResourcesKeys.Required_FieldName.Localize(SharedResourcesKeys.Branch.Localize()))
                .WithStatusCode(HttpStatusCode.BadRequest);

        IResult<Address>? addressCreateResult = null;

        if (address != null)
        {
            addressCreateResult = Address.Create(address.Value.street, address.Value.city, address.Value.state, address.Value.country, address.Value.postalCode, address.Value.comment);
            if (addressCreateResult.IsFailed)
                return addressCreateResult.ChangeType(new Employee());
        }

        List<PhoneNumber>? phoneNumbersToBeCreated = null;

        if (phoneNumbers != null)
        {
            phoneNumbersToBeCreated = new List<PhoneNumber>();
            foreach (var (number, comment) in phoneNumbers)
            {
                var phoneNumberCreateResult = PhoneNumber.Create(number, comment);
                if (phoneNumberCreateResult.IsFailed)
                    return phoneNumberCreateResult.ChangeType(new Employee());

                phoneNumbersToBeCreated.Add(phoneNumberCreateResult.Value);
            }
        }

        return new Result<Employee>(new Employee(employeeId, branchId, addressCreateResult?.Value, phoneNumbersToBeCreated));
    }
}