using Domain;
using Domain.Identity;
using Domain.Relations;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<ShoppingCart> ShoppingCarts { get; set; }
    public DbSet<ProductInOrder> ProductInOrders { get; set; }
    public DbSet<ProductInShoppingCart> ProductInShoppingCarts { get; set; }
    public DbSet<Email> Emails { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<User>()
            .HasIndex(user => user.Username);

        modelBuilder.Entity<User>()
            .HasIndex(user => user.Email);

        // modelBuilder.Entity<User>()
        //     .Property(user => user.Role);
        
        modelBuilder.Entity<ShoppingCart>()
            .HasOne<User>(item => item.User)
            .WithOne(item => item.ShoppingCart)
            .HasForeignKey<ShoppingCart>(item => item.UserId);

        modelBuilder.Entity<ProductInShoppingCart>()
            .HasKey(item => new { item.ProductId, item.ShoppingCartId });

        modelBuilder.Entity<ProductInShoppingCart>()
            .HasOne(item => item.Product)
            .WithMany(item => item.ProductInShoppingCarts)
            .HasForeignKey(item => item.ProductId);
        
        modelBuilder.Entity<ProductInShoppingCart>()
            .HasOne(item => item.ShoppingCart)
            .WithMany(item => item.ProductInShoppingCarts)
            .HasForeignKey(item => item.ShoppingCartId);

        modelBuilder.Entity<ProductInOrder>()
            .HasKey(item => new { item.OrderId, item.ProductId });

        modelBuilder.Entity<ProductInOrder>()
            .HasOne(item => item.Order)
            .WithMany(item => item.ProductInOrders)
            .HasForeignKey(item => item.OrderId);
        
        modelBuilder.Entity<ProductInOrder>()
            .HasOne(item => item.Product)
            .WithMany(item => item.ProductInOrders)
            .HasForeignKey(item => item.ProductId);
    }
}