using Domain;

namespace Repository.Interface;

public interface IShoppingCartRepository
{
    ShoppingCart GetByUserId(int? id);
    ShoppingCart GetByUsername(string? username);
    void Update(ShoppingCart shoppingCart);
    
}