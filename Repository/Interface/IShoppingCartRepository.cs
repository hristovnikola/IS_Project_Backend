using Domain;

namespace Repository.Interface;

public interface IShoppingCartRepository
{
    ShoppingCart GetByUserId(int? userId);
    
}