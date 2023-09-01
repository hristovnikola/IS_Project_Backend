namespace Domain.Relations;

public class ProductInShoppingCart
{
    public int ProductId { get; set; }
    public Product Product { get; set; }
    public int ShoppingCartId { get; set; }
    public ShoppingCart ShoppingCart { get; set; }
    public int Quantity { get; set; }
}