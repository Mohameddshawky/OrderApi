using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Customer> Customers { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<OrderItem> OrderItems { get; set; } = null!;
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<Coupon> Coupons { get; set; } = null!;
    public DbSet<OrderStatusHistory> OrderStatusHistories { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Customer>()
            .HasMany(c => c.Orders)
            .WithOne(o => o.Customer)
            .HasForeignKey(o => o.CustomerId);
            
        modelBuilder.Entity<Order>()
            .HasMany(o => o.OrderItems)
            .WithOne(oi => oi.Order)
            .HasForeignKey(oi => oi.OrderId);
            
        modelBuilder.Entity<Order>()
            .HasOne(o => o.Coupon)
            .WithMany()
            .HasForeignKey(o => o.CouponId);
            
        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Product)
            .WithMany()
            .HasForeignKey(oi => oi.ProductId);
            
        modelBuilder.Entity<Customer>()
            .HasQueryFilter(c => c.IsActive);
            
        modelBuilder.Entity<Product>()
            .HasQueryFilter(p => p.IsActive);

        modelBuilder.Entity<User>().HasData(new User
        {
            Id = 1,
            Email = "admin@ordersystem.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("1234"),
            Role = "Admin"
        });
    }
}

