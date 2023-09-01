using Domain.Relations;

namespace Repository.Interface;

public interface IProductInShoppingCartRepository
{
    IEnumerable<ProductInShoppingCart> GetAll();
    ProductInShoppingCart Get(int? id);
    void Insert(ProductInShoppingCart entity);
    bool Update(ProductInShoppingCart entity);
    void Delete(ProductInShoppingCart entity);
    bool ProductExists(int id);
}