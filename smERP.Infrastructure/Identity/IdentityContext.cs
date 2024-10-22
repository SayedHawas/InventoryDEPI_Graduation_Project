using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using smERP.Domain.Entities.ExternalEntities;
using smERP.Domain.Entities.Product;
using smERP.Domain.Entities.User;
using smERP.Domain.ValueObjects;
using smERP.Infrastructure.Identity.Configurations;
using smERP.Infrastructure.Identity.Models.Users;
using System.Reflection;

namespace smERP.Infrastructure.Identity;

public class IdentityContext(DbContextOptions<IdentityContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new ApplicationUserConfiguration());
        modelBuilder.ApplyConfiguration(new RoleClaimConfiguration());
        modelBuilder.ApplyConfiguration(new RoleConfiguration());
        modelBuilder.ApplyConfiguration(new UserClaimConfiguration());
        modelBuilder.ApplyConfiguration(new UserLoginConfiguration());
        modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
        modelBuilder.ApplyConfiguration(new UserTokenConfiguration());

        //modelBuilder.Ignore<Address>();
        //modelBuilder.Ignore<ProductInstanceAttributeValue>();
        //modelBuilder.Ignore<ProductSupplier>();
        //modelBuilder.Ignore<StoredProductInstance>();
        //modelBuilder.Ignore<BilingualName>();


        //modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly);
        //modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}