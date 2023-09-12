using Domain;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;

    public OrderService(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }
    public ICollection<Order> getAllOrders()
    {
        return this._orderRepository.getAllOrders();
    }

    public Order getOrderDetails(int id)
    {
        return this._orderRepository.getOrderDetails(id);
    }
}