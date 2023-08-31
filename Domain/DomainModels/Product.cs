using System.ComponentModel.DataAnnotations;
using Domain.Relations;

namespace Domain;

public class Product : BaseEntity
{
    public string Name { get; set; }
    public string ImagePath { get; set; }
    public string Description { get; set; }
    public double Price { get; set; }
    
    public new int Id
    {
        get { return base.Id; }
        set { base.Id = value; }
    }
    
    public ICollection<ProductInShoppingCart> ProductInShoppingCarts { get; set; }
    public ICollection<ProductInOrder> ProductInOrders { get; set; }
}