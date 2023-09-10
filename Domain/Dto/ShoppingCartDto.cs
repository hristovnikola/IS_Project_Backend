using Domain.Relations;

namespace Domain.Dto;

public class ShoppingCartDto
{
    public ICollection<ProductInShoppingCart> Products { get; set; }

    public double TotalPrice { get; set; }
}