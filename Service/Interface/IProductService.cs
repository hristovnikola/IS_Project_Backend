using Domain;
using Domain.Dto;

namespace Service.Interface;

public interface IProductService
{
    ICollection<Product> GetAllProducts();
    Product GetProduct(int? id);
    bool CreateProduct(Product product);
    bool UpdateProduct(Product product);
    void DeleteProduct(int id);
    bool ProductExists(int id);
    AddToShoppingCardDto GetShoppingCartInfo(int? id);
    bool AddToShoppingCart(int userId, AddToShoppingCardDto item);
    
    // void AttachProduct(Product product);

    // AddToShoppingCardDto GetShoppingCartInfo(Guid? id);
    // bool AddToShoppingCart(AddToShoppingCardDto item, string userID);
}