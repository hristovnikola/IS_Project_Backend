using Domain.Relations;
using Repository.Interface;

namespace Repository.Implementation;

public class ProductInShoppingCartRepository : IProductInShoppingCartRepository
{
    private readonly AppDbContext _context;

    public ProductInShoppingCartRepository(AppDbContext context)
    {
        _context = context;
    }

    public IEnumerable<ProductInShoppingCart> GetAll()
    {
        return _context.ProductInShoppingCarts.ToList();
    }

    public ProductInShoppingCart Get(int? id)
    {
        return _context.ProductInShoppingCarts.Find(id);
    }

    public void Insert(ProductInShoppingCart entity)
    {
        _context.ProductInShoppingCarts.Add(entity);
        _context.SaveChanges();
    }

    public bool Update(ProductInShoppingCart entity)
    {
        _context.ProductInShoppingCarts.Update(entity);
        var saved = _context.SaveChanges();
        return saved > 0 ? true : false;
    }

    public void Delete(ProductInShoppingCart entity)
    {
        _context.ProductInShoppingCarts.Remove(entity);
        _context.SaveChanges();
    }

    public bool ProductExists(int id)
    {
        return _context.ProductInShoppingCarts.Any(e => e.ProductId == id);
    }
}