
namespace smERP.Domain.ValueObjects;

public class ContactPerson : ValueObject
{
    public string Name { get; private set; } = null!;
    public string Position { get; private set; } = null!;
    public PhoneNumber PhoneNumber { get; private set; } = null!;
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Name;
    }
}
