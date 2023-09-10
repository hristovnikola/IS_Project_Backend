using Domain;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;

namespace Repository.Implementation;

public class ShoppingCartRepository : IShoppingCartRepository
{
    private readonly AppDbContext _dbContext;
    private DbSet<ShoppingCart> entities;

    public ShoppingCartRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
        entities = _dbContext.Set<ShoppingCart>();
    }

    public ShoppingCart GetByUserId(int? userId)
    {
        return entities.Include(sc => sc.ProductInShoppingCarts)
            .ThenInclude(pisc => pisc.Product)
            .SingleOrDefault(sc => sc.UserId == userId);
    }
    
    public ShoppingCart GetByUsername(string? username)
    {
        return entities.Include(sc => sc.ProductInShoppingCarts)
            .ThenInclude(pisc => pisc.Product)
            .SingleOrDefault(sc => sc.User.Username == username);
    }

    public void Update(ShoppingCart shoppingCart)
    {
        _dbContext.ShoppingCarts.Update(shoppingCart);
        _dbContext.SaveChanges();
    }
}