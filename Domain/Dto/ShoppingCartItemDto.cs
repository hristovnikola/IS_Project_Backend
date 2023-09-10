namespace Domain.Dto;

public class ShoppingCartItemDto
{
    public int ProductId { get; set; }
    public string Name { get; set; }
    public string ImagePath { get; set; }
    public string Description { get; set; }
    public double Price { get; set; }
    public int Quantity { get; set; }
    public double TotalPrice { get; set; }
}