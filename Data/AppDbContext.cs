using Microsoft.EntityFrameworkCore;
using BlazorApp.Models;

namespace BlazorApp.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<User> Users => Set<User>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 种子数据：原本写死在 ProductService 里的 8 个商品，迁移时自动写进数据库
        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Mechanical Keyboard Pro", Price = 189, OriginalPrice = 249, Category = "Electronics", Rating = 4.8, Reviews = 312, Badge = "Sale" },
            new Product { Id = 2, Name = "Noise Cancelling Headphones", Price = 299, Category = "Electronics", Rating = 4.9, Reviews = 876 },
            new Product { Id = 3, Name = "Minimalist Watch", Price = 159, OriginalPrice = 199, Category = "Accessories", Rating = 4.6, Reviews = 234, Badge = "Sale" },
            new Product { Id = 4, Name = "Leather Wallet", Price = 59, Category = "Accessories", Rating = 4.7, Reviews = 445 },
            new Product { Id = 5, Name = "Wireless Charger", Price = 45, Category = "Electronics", Rating = 4.5, Reviews = 189, Badge = "New" },
            new Product { Id = 6, Name = "Canvas Backpack", Price = 89, Category = "Bags", Rating = 4.8, Reviews = 567 },
            new Product { Id = 7, Name = "Sunglasses UV400", Price = 75, OriginalPrice = 99, Category = "Accessories", Rating = 4.4, Reviews = 203, Badge = "Sale" },
            new Product { Id = 8, Name = "Running Shoes", Price = 129, Category = "Footwear", Rating = 4.7, Reviews = 891 }
        );
    }
}