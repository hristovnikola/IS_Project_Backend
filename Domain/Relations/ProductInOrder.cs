namespace Domain.Relations;

public class ProductInOrder
{
    public int OrderId { get; set; }
    public Order Order { get; set; }

    public int ProductId { get; set; }
    public Product Product { get; set; }
    
    public int Quantity { get; set; }
}