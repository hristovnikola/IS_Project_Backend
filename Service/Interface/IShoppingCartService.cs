using Domain.Dto;

namespace Service.Interface;

public interface IShoppingCartService
{
    // ShoppingCartDto getShoppingCartInfo(string userId);
    // bool deleteProductFromSoppingCart(string userId, Guid productId);
    // bool order(string userId);
    
    ShoppingCartDto getShoppingCartInfo(int userId);
    bool deleteProductFromSoppingCart(int userId, int productId);
    bool increaseProductQuantity(int userId, int productId);
    bool decreaseProductQuantity(int userId, int productId);
    bool order(int userId);
}