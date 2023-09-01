using Domain.Identity;
using Domain.Relations;

namespace Domain;

public class ShoppingCart : BaseEntity
{
    public int UserId { get; set; }
    public User User { get; set; }
    public ICollection<ProductInShoppingCart> ProductInShoppingCarts { get; set; }
}