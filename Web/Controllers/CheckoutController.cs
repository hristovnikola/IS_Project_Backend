using AutoMapper;
using Domain;
using Domain.Dto;
using Microsoft.AspNetCore.Mvc;
using Repository.Interface;
using Service.Interface;
using Stripe.Checkout;

namespace Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CheckoutController : Controller
{
    private readonly IProductService _productService;
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;
    private readonly IShoppingCartService _shoppingCartService;
    
    public CheckoutController(IProductService productService,  IMapper mapper, IUserRepository userRepository, IShoppingCartService shoppingCartService)
    {
        _productService = productService;
        _mapper = mapper;
        _userRepository = userRepository;
        _shoppingCartService = shoppingCartService;
    }

    [HttpGet("checkout/success/{userId}")]
    [ProducesResponseType(301)]
    public IActionResult SuccessfulCheckout(int userId)
    {
        _shoppingCartService.order(userId);

        return Redirect("http://localhost:3000/cart");
    }
    
    [HttpGet("checkout/failure")]
    [ProducesResponseType(301)]
    public IActionResult UnsuccessfulCheckout()
    {
        return Redirect("http://localhost:3000/cart");
    }
    
    [HttpPost("checkout/")]
    public ActionResult CreateCheckoutSession()
    {
        var username = User.Identity?.Name;
        int userId = _userRepository.GetUserIdByUsername(username);
    
        var shoppingCartInfo = _shoppingCartService.getShoppingCartInfo(userId);

        var shoppingCart = _mapper.Map<ShoppingCartForLoggedInUserDto>(shoppingCartInfo);

        var products = shoppingCart.Products;
        
        var options = new SessionCreateOptions()
        {
            SuccessUrl = $"https://localhost:7051/api/Checkout/checkout/success/{userId}",
            CancelUrl = "https://localhost:7051/api/Checkout/checkout/failure",
            LineItems = products.Select(product => new SessionLineItemOptions
            {
               PriceData = new SessionLineItemPriceDataOptions
               {
                   UnitAmount = Convert.ToInt32(product.Price * 100),
                   Currency = "USD",
                   ProductData = new SessionLineItemPriceDataProductDataOptions
                   {
                       Name = product.Name,
                   }
               }, Quantity = product.Quantity
            }).ToList(),
            PaymentMethodTypes = new List<string>()
            {
                "card"
            },
            Mode = "payment",
        };

        var service = new SessionService();
        Session session = service.Create(options);

        var publishableKey =
            "pk_test_51NNc7MJYxaVpIsNPsejTvjlgTvs0QiY0oroPNPGRWl1vnEbTJSMR9LQr9kBAHozJEKHEbGUkXWxN6FY2lriz1fYt00e5JHaUCw";
        
        Response.Headers.Add("Location", session.Url);
        return Ok(new CheckoutOrderResponse
        {
            SessionId = session.Id,
            PubKey = publishableKey
        });
    }
}