using Domain;
using Domain.Relations;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;

namespace Repository.Implementation;

public class ProductInOrderRepository : IProductInOrderRepository
{
    private readonly AppDbContext _context;
    private DbSet<ProductInOrder> entities;

    public ProductInOrderRepository(AppDbContext context)
    {
        _context = context;
        entities = _context.Set<ProductInOrder>();
    }
    
    public void Insert(ProductInOrder entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException("entity");
        }
        entities.Add(entity);
        _context.SaveChanges();
    }
}