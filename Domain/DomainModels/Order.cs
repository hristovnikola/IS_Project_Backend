using Domain.Identity;
using Domain.Relations;

namespace Domain;

public class Order : BaseEntity
{
    public int UserId { get; set; }
    public User User { get; set; }

    public ICollection<ProductInOrder> ProductInOrders { get; set; }
}