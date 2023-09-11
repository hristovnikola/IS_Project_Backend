using Domain.Identity;
using Domain.Relations;

namespace Domain;

public class Order : BaseEntity
{
    public int UserId { get; set; }
    public User User { get; set; }
    
    public new int Id
    {
        get { return base.Id; }
        set { base.Id = value; }
    }

    public ICollection<ProductInOrder> ProductInOrders { get; set; }
}