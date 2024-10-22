using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using smERP.Infrastructure.Identity.Models.Users;

namespace smERP.Infrastructure.Identity.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable(name: "Users");

        builder.OwnsOne(u => u.Employee, employee =>
        {
            employee.ToTable("Employees");
            employee.WithOwner().HasForeignKey(e => e.Id);

            employee.OwnsMany(p => p.PhoneNumbers, phoneNumber =>
            {
                phoneNumber.ToTable("EmployeePhoneNumbers");
                phoneNumber.WithOwner();
                phoneNumber.Property(n => n.CountryCode).IsRequired();
                phoneNumber.Property(n => n.Number).IsRequired();
                phoneNumber.Property(n => n.Comment);
            });

            employee.OwnsOne(p => p.Address, address =>
            {
                address.WithOwner();
                address.HasKey(x => x.Id);
                address.ToTable("EmployeeAddresses");
                address.Property(n => n.Country).IsRequired();
                address.Property(n => n.State).IsRequired();
                address.Property(n => n.City).IsRequired();
                address.Property(n => n.Street).IsRequired();
                address.Property(n => n.PostalCode).IsRequired();
                address.Property(n => n.Comment);
            });

            employee.Ignore(x => x.Branch);
        });
    }
}