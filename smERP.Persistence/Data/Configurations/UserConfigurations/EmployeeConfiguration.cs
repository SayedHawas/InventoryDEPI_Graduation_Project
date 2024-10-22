using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using smERP.Domain.Entities.User;
using System.Reflection.Emit;

namespace smERP.Persistence.Data.Configurations.UserConfigurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employees");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.OwnsMany(p => p.PhoneNumbers, phoneNumber =>
        {
            phoneNumber.ToTable("EmployeePhoneNumbers");
            phoneNumber.WithOwner();
            phoneNumber.Property(n => n.CountryCode).IsRequired();
            phoneNumber.Property(n => n.Number).IsRequired();
            phoneNumber.Property(n => n.Comment);
        });

        builder.OwnsOne(p => p.Address, address =>
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
    }
}
