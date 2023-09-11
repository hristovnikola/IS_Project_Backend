using Domain.Identity;

namespace Domain.Dto;

public class OrderDto
{
    public int Id { get; set; }
    public UserForOrderDto User { get; set; }
    public List<ProductDto> ProductInOrders { get; set; }
}