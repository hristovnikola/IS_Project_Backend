using Domain.Dto;

namespace Service.Interface;

public interface IShoppingCartService
{
    // ShoppingCartDto getShoppingCartInfo(string userId);
    // bool deleteProductFromSoppingCart(string userId, Guid productId);
    // bool order(string userId);
    
    ShoppingCartDto getShoppingCartInfo();
    bool deleteProductFromSoppingCart(int productId);
    bool order(string userId);
}