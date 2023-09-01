using Domain;

namespace Repository.Interface;

public interface IOrderRepository
{
    public List<Order> getAllOrders();
    public Order getOrderDetails(BaseEntity model);
}