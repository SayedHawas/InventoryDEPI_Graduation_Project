using Microsoft.EntityFrameworkCore;
using smERP.Domain.Entities.Organization;

namespace smERP.Persistence.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Company> Companies { get; set; }
}
