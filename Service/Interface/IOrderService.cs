using Domain;

namespace Service.Interface;

public interface IOrderService
{
    public ICollection<Order> getAllOrders();
    public Order getOrderDetails(int id);
}