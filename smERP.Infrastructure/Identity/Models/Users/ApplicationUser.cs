using Microsoft.AspNetCore.Identity;
using smERP.Domain.Entities.User;

namespace smERP.Infrastructure.Identity.Models.Users;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public virtual Employee Employee { get; set; } = new Employee();
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime RefreshTokenExpiration { get; set; } = DateTime.MinValue;
    public bool IsAccountDisabled { get; set; } = false;

    public void GenerateRefreshToken()
    {
        RefreshToken = Guid.NewGuid().ToString();
        RefreshTokenExpiration = DateTime.UtcNow.AddDays(30);
    }

    public void AddBranch(int branchId)
    {
        Employee.BranchId = branchId;
    }

    public void AddAddress(string country, string city, string state, string street, string postalCode, string? comment)
    {
        Employee.AddAddress(country, city, state, street, postalCode, comment);
    }

    public bool IsRefreshTokenValid(string token)
    {
        return RefreshToken == token && RefreshTokenExpiration > DateTime.UtcNow;
    }

    public bool IsExistingRefreshTokenValid()
    {
        return RefreshTokenExpiration > DateTime.UtcNow;
    }

    public void RevokeRefreshToken()
    {
        RefreshToken = null;
        RefreshTokenExpiration = DateTime.MinValue;
    }
}