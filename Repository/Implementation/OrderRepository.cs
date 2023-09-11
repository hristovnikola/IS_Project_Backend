using Domain;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;

namespace Repository.Implementation;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;
    private DbSet<Order> entities;

    public OrderRepository(AppDbContext context)
    {
        _context = context;
        entities = context.Set<Order>();
    }

    public ICollection<Order> getAllOrders()
    {
        return entities
            .Include(z => z.User)
            .Include(z => z.ProductInOrders)
            .ThenInclude(po => po.Product)
            .ToListAsync().Result;
    }

    public Order getOrderDetails(BaseEntity model)
    {
        return entities
            // .Include(z => z.User)
            .Include(z => z.ProductInOrders)
            .Include("ProductInOrders.Product")
            .SingleOrDefaultAsync(z => z.Id == model.Id).Result;
    }
}