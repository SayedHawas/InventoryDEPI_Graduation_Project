using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using smERP.Application.Contracts.Persistence;
using smERP.Persistence.Data;
using smERP.Persistence.Managers;
using smERP.Persistence.Repositories;

namespace smERP.Persistence;

public static class PersistenceDependencies
{
    public static IServiceCollection AddPersistenceDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        //services.AddDbContext<AppDbContext>(options =>
        //  options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
        //  b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

        services.AddDbContext<ProductDbContext>(options =>
          options.UseSqlServer(configuration.GetConnectionString("ProductConnection"),
          b => b.MigrationsAssembly(typeof(ProductDbContext).Assembly.FullName))
          .EnableSensitiveDataLogging());

        var root = Path.Combine(Directory.GetCurrentDirectory(), "FileStorage");

        services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
        services.AddTransient<IUnitOfWork, UnitOfWork>()
                //.AddTransient<ICompanyRepository, CompanyRepository>()
                .AddTransient<ICategoryRepository, CategoryRepository>()
                .AddTransient<IBrandRepository, BrandRepository>()
                .AddTransient<IAttributeRepository, AttributeRepository>()
                .AddTransient<IProductRepository, ProductRepository>()
                .AddTransient<ISupplierRepository, SupplierRepository>()
                .AddTransient<IBranchRepository, BranchRepository>()
                .AddTransient<IProcurementTransactionRepository, ProcurementTransactionRepository>()
                .AddTransient<IStorageLocationRepository, StorageLocationRepository>()
                .AddTransient<IFileStorageRepository, FileStorageRepository>()
                .AddTransient<INotificationRepository, NotificationRepository>()
                .AddSingleton(new FileStorageManager(root));

        return services;
    }
}
