using Domain;

namespace Repository.Interface;

public interface IOrderRepository
{
    public ICollection<Order> getAllOrders();
    public Order getOrderDetails(int id);
}