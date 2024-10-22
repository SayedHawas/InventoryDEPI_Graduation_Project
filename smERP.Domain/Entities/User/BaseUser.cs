using Microsoft.AspNetCore.Identity;
using smERP.Domain.ValueObjects;

namespace smERP.Domain.Entities.User;
public class BaseUser : IdentityUser
{
    public Address? UserAddress { get; set; }
    public int? TotalAmountOwedInCents { get; set; }
}
