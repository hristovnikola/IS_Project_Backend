using Domain;
using Domain.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation;

public class ProductService : IProductService
{
    private readonly IRepository<Product> _productRepository;
    private readonly ILogger<ProductService> _logger;

    public ProductService(IRepository<Product> productRepository, ILogger<ProductService> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public ICollection<Product> GetAllProducts()
    {
        return this._productRepository.GetAll().ToList();
    }

    public Product GetProduct(int? id)
    {
        return this._productRepository.Get(id);
    }

    public bool CreateProduct(Product p)
    {
        try
        {
            _productRepository.Insert(p);
            return true;
        }
        catch (Exception ex)
        {
            return false; 
        }
    }

    public bool UpdateProduct(Product p)
    {
        return _productRepository.Update(p);
    }

    public void DeleteProduct(int id)
    {
        var product = GetProduct(id);
        _productRepository.Delete(product);
    }

    public bool ProductExists(int id)
    {
        return this._productRepository.ProductExists(id);
    }

    // public void AttachProduct(Product product)
    // {
    //     _productRepository.AttachProduct(product);
    // }
}