
using smERP.Domain.ValueObjects;

namespace smERP.Domain.Entities.User;

public abstract class Person
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public Email Email { get; private set; }
    public ICollection<PhoneNumber> PhoneNumbers { get; private set; }
    //public ICollection<Address> Addresses { get; private set; }
}
