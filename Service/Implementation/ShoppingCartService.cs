using System.Text;
using Domain;
using Domain.Dto;
using Domain.Relations;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation;

public class ShoppingCartService : IShoppingCartService
{
    private readonly IShoppingCartRepository _shoppingCartRepository;

    private readonly IRepository<Order> _orderRepository;

    // private readonly IRepository<ProductInOrder> _productInOrderRepository;
    private readonly IUserRepository _userRepository;
    private readonly IProductInOrderRepository _productInOrderRepository;
    private readonly IConfiguration _configuration;
    private readonly IEmailRepository _emailRepository;

    public ShoppingCartService(IShoppingCartRepository shoppingCartRepository, IUserRepository userRepository,
        IRepository<Order> orderRepository, IProductInOrderRepository productInOrderRepository, IConfiguration configuration, IEmailRepository emailRepository)
    {
        _shoppingCartRepository = shoppingCartRepository;
        _userRepository = userRepository;
        _orderRepository = orderRepository;
        _productInOrderRepository = productInOrderRepository;
        _configuration = configuration;
        _emailRepository = emailRepository;
        // _productInOrderRepository = productInOrderRepository;
        // _mailRepository = mailRepository;
    }


    public ShoppingCartDto getShoppingCartInfo(int userId)
    {
        if(userId != null)
        {
            var userCard = _shoppingCartRepository.GetByUserId(userId);
            
            var allProducts = userCard.ProductInShoppingCarts.ToList();

            var allProductPrices = allProducts?.Select(z => new
            {
                ProductPrice = z.Product?.Price ?? 0, 
                Quantity = z.Quantity
            }).ToList();
            
            double totalPrice = 0.0;

            foreach (var item in allProductPrices)
            {
                totalPrice += item.Quantity * item.ProductPrice;
            }

            var reuslt = new ShoppingCartDto
            {
                Products = allProducts,
                TotalPrice = totalPrice
            };

            return reuslt;
        }
        return new ShoppingCartDto();
    }

    public bool deleteProductFromSoppingCart(int userId, int productId)
    {
        if(userId != null && productId != null)
        {
            var loggedInUser = this._userRepository.GetById(userId);

            var userShoppingCart = _shoppingCartRepository.GetByUserId(userId);

            var itemToDelete = userShoppingCart.ProductInShoppingCarts.Where(z => z.ProductId.Equals(productId)).FirstOrDefault();

            userShoppingCart.ProductInShoppingCarts.Remove(itemToDelete);

            this._shoppingCartRepository.Update(userShoppingCart);

            return true;
        }
        return false;
    }

    public bool increaseProductQuantity(int userId, int productId)
    {
        if (userId != 0 && productId != 0)
        {
            var userShoppingCart = _shoppingCartRepository.GetByUserId(userId);

            if (userShoppingCart != null)
            {
                var productInCart = userShoppingCart.ProductInShoppingCarts
                    .FirstOrDefault(p => p.ProductId == productId);

                if (productInCart != null)
                {
                    productInCart.Quantity++;
                
                    _shoppingCartRepository.Update(userShoppingCart);

                    return true;
                }
            }
        }

        return false;
    }

    public bool decreaseProductQuantity(int userId, int productId)
    {
        if (userId != 0 && productId != 0)
        {
            var userShoppingCart = _shoppingCartRepository.GetByUserId(userId);

            if (userShoppingCart != null)
            {
                var productInCart = userShoppingCart.ProductInShoppingCarts
                    .FirstOrDefault(p => p.ProductId == productId);

                if (productInCart != null)
                {
                    productInCart.Quantity--;
                
                    _shoppingCartRepository.Update(userShoppingCart);

                    return true;
                }
            }
        }

        return false;
    }

    public bool order(int userId)
        {
            if (userId != null)
            {
                var loggedInUser = this._userRepository.GetById(userId);
                var userCard = _shoppingCartRepository.GetByUserId(userId);

                Order order = new Order
                {
                    User = loggedInUser,
                    UserId = userId
                };

                this._orderRepository.Insert(order);

                List<ProductInOrder> productInOrders = new List<ProductInOrder>();

                var result = userCard.ProductInShoppingCarts.Select(z => new ProductInOrder
                {
                    ProductId = z.Product.Id,
                    Product = z.Product,
                    OrderId = order.Id,
                    Order = order, 
                    Quantity = z.Quantity
                }).ToList();

                StringBuilder sb = new StringBuilder();

                var totalPrice = 0.0;

                sb.AppendLine("Your order is completed. The order conatins: ");

                for (int i = 1; i <= result.Count(); i++)
                {
                    var currentItem = result[i - 1];
                    totalPrice += currentItem.Quantity * currentItem.Product.Price;
                    sb.AppendLine(i.ToString() + ". " + currentItem.Product.Name + " with quantity of: " + currentItem.Quantity + " and price of: $" + currentItem.Product.Price);
                }

                sb.AppendLine("Total price for your order: " + totalPrice.ToString());

                var host = _configuration.GetSection("EmailSettings:Host").Value;
                var userName = _configuration.GetSection("EmailSettings:UserName").Value;
                var password = _configuration.GetSection("EmailSettings:Password").Value;

                var message = new MimeMessage();
                message.From.Add(MailboxAddress.Parse(userName));
                message.To.Add(MailboxAddress.Parse(loggedInUser.Email));
                message.Subject = "Online PC Store Order";
                message.Body = new TextPart(TextFormat.Text) { Text = sb.ToString()};

                using var smtpClient = new SmtpClient();
                smtpClient.Connect(host, 587, SecureSocketOptions.StartTls);
                smtpClient.Authenticate(userName, password);
                smtpClient.Send(message);
                smtpClient.Disconnect(true);

                Email email = new Email()
                {
                    To = loggedInUser.Email,
                    Subject = "Online PC Store Order",
                    Body = sb.ToString()
                };
                
                _emailRepository.Create(email);

                productInOrders.AddRange(result);

                foreach (var item in productInOrders)
                {
                    this._productInOrderRepository.Insert(item);
                }

                userCard.ProductInShoppingCarts.Clear();

                this._userRepository.Update(loggedInUser);
                // this._mailRepository.Insert(mail);

                return true;
            }

            return false;
        }
}