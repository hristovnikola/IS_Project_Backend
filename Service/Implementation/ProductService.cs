using Domain;
using Domain.Dto;
using Domain.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Repository.Interface;
using Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Service.Implementation;

public class ProductService : IProductService
{
    private readonly IRepository<Product> _productRepository;
    private readonly IProductInShoppingCartRepository _productInShoppingCartRepository;
    private readonly IUserRepository _userRepository;
    private readonly IShoppingCartRepository _shoppingCartRepository;

    public ProductService(IRepository<Product> productRepository,
        IProductInShoppingCartRepository productInShoppingCartRepository, IUserRepository userRepository,
        IShoppingCartRepository shoppingCartRepository)
    {
        _productRepository = productRepository;
        _userRepository = userRepository;
        _productInShoppingCartRepository = productInShoppingCartRepository;
        _shoppingCartRepository = shoppingCartRepository;
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

    public AddToShoppingCardDto GetShoppingCartInfo(int? id)
    {
        
        var product = this.GetProduct(id);
        AddToShoppingCardDto model = new AddToShoppingCardDto
        {
            // SelectedProduct = product,
            SelectedProductId = product.Id,
            Quantity = 1
        };

        return model;
    }

    public bool AddToShoppingCart(int userId, AddToShoppingCardDto item)
    {
        // var user = this._userRepository.GetById(item.UserId);

        var userShoppingCard = _shoppingCartRepository.GetByUserId(userId);

        if (item.SelectedProductId != null && userShoppingCard != null)
        {
            var product = this.GetProduct(item.SelectedProductId);
            //{896c1325-a1bb-4595-92d8-08da077402fc}

            if (product != null)
            {
                ProductInShoppingCart itemToAdd = new ProductInShoppingCart
                {
                    Product = product,
                    ProductId = product.Id,
                    ShoppingCart = userShoppingCard,
                    ShoppingCartId = userShoppingCard.Id,
                    Quantity = item.Quantity
                };

                if (userShoppingCard.ProductInShoppingCarts != null)
                {
                    var existing = userShoppingCard.ProductInShoppingCarts.Where(z =>
                        z.ShoppingCartId == userShoppingCard.Id && z.ProductId == itemToAdd.ProductId).FirstOrDefault();
                    if (existing != null)
                    {
                        existing.Quantity += itemToAdd.Quantity;
                        this._productInShoppingCartRepository.Update(existing);
                    }
                    else
                    {
                        this._productInShoppingCartRepository.Insert(itemToAdd);
                    }
                }
                else
                {
                    this._productInShoppingCartRepository.Insert(itemToAdd);
                }


                return true;
            }

            return false;
        }

        return false;
    }
}