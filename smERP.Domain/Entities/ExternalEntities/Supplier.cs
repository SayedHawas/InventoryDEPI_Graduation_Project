using smERP.Domain.Entities.Product;
using smERP.Domain.ValueObjects;
using smERP.SharedKernel.Responses;

namespace smERP.Domain.Entities.ExternalEntities;

public sealed class Supplier : ExternalEntity, IAggregateRoot
{
    public IReadOnlyCollection<ProductSupplier> SuppliedProducts { get; private set; } = new List<ProductSupplier>();

    private Supplier(BilingualName name, List<Address> addresses) : base(name, addresses)
    {
    }

    private Supplier() { }

    public static IResult<Supplier> Create(string englishName, string arabicName, List<(string street, string city, string state, string country, string postalCode, string? comment)> addresses)
    {
        var createResult = CreateBaseDetails(englishName, arabicName, addresses);
        if (createResult.IsFailed)
            return createResult.ChangeType(new Supplier());

        return new Result<Supplier>(new Supplier(createResult.Value.Name, createResult.Value.Addresses));
    }
}
