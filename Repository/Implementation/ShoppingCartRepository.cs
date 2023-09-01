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
            .SingleOrDefault(sc => sc.UserId == userId);
    }
}