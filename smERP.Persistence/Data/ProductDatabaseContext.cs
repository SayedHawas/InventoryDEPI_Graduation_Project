using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using smERP.Domain.Entities;
using smERP.Domain.Entities.Product;
using smERP.Persistence.Data.Configurations;
using smERP.Persistence.Data.Interceptors;
using Attribute = smERP.Domain.Entities.Product.Attribute;
using smERP.Domain.Entities.ExternalEntities;
using smERP.Domain.Entities.InventoryTransaction;
using smERP.Domain.Entities.Organization;
using smERP.Persistence.Outbox;
using smERP.Domain.Entities.User;
using smERP.Application.Notifications;

namespace smERP.Persistence.Data;

public class ProductDbContext(DbContextOptions<ProductDbContext> options) : DbContext(options)
{

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(new CentralInterceptor());
        //optionsBuilder.AddInterceptors(new SoftDeleteInterceptor());
        //optionsBuilder.AddInterceptors(new ChangeLogInterceptor());
        //optionsBuilder.AddInterceptors(new ConvertDomainEventsToOutboxMessagesInterceptor());
    }

    public virtual DbSet<Attribute> Attributes { get; set; }

    //public virtual DbSet<AttributeValue> AttributeValues { get; set; }

    public virtual DbSet<Brand> Brands { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    //public virtual DbSet<DuplicateProductInstance> DuplicateProductInstances { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductInstance> ProductInstances { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<StorageLocation> StorageLocations { get; set; }

    public virtual DbSet<Branch> Branches { get; set; }

    public virtual DbSet<ProcurementTransaction> ProcurementTransactions { get; set; }

    public virtual DbSet<SalesTransaction> SalesTransactions { get; set; }

    public virtual DbSet<AdjustmentTransaction> AdjustmentTransactions { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    //public virtual DbSet<ProductInstanceAttribute> ProductInstanceAttributes { get; set; }

    public virtual DbSet<ChangeLog> ChangeLogs { get; set; }

    public virtual DbSet<OutboxMessage> OutboxMessages { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
