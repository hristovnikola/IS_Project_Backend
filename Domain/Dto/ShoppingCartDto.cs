using Domain.Relations;

namespace Domain.Dto;

public class ShoppingCartDto
{
    public List<ProductInShoppingCart> Products { get; set; }

    public double TotalPrice { get; set; }
}