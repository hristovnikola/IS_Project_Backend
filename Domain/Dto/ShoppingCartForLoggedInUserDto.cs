namespace Domain.Dto;

public class ShoppingCartForLoggedInUserDto
{
    public List<ShoppingCartItemDto> Products { get; set; }
    public double TotalPrice { get; set; }
}