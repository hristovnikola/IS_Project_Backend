using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Domain.Dto;
using Microsoft.AspNetCore.Authorization;
using Repository.Interface; // Import your DTOs
using Service.Interface; // Import your service interface

namespace Web.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public ShoppingCartController(IProductService productService, IShoppingCartService shoppingCartService,
            IUserRepository userRepository, IMapper mapper)
        {
            _productService = productService;
            _shoppingCartService = shoppingCartService;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [HttpGet("shopping-cart-info/")]
        [ProducesResponseType(200, Type = typeof(ShoppingCartForLoggedInUserDto))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetShoppingCartInfo()
        {
            var username = User.Identity?.Name;
            int userId = _userRepository.GetUserIdByUsername(username);

            var shoppingCartInfo = _shoppingCartService.getShoppingCartInfo(userId);

            if (shoppingCartInfo == null)
            {
                return NotFound();
            }

            var shoppingCartDto = _mapper.Map<ShoppingCartForLoggedInUserDto>(shoppingCartInfo);

            return Ok(shoppingCartDto);
        }


        [HttpDelete("remove-product/{productId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [Authorize("UserPolicy")]
        public IActionResult RemoveProductFromShoppingCart(int productId)
        {
            var username = User.Identity?.Name;
            int userId = _userRepository.GetUserIdByUsername(username);

            var removedProduct = _shoppingCartService.deleteProductFromSoppingCart(userId, productId);

            if (removedProduct)
            {
                return Ok("Product removed from the shopping cart successfully.");
            }
            else
            {
                ModelState.AddModelError("", "Unable to delete the product.");
                return BadRequest(ModelState);
            }
        }

        [HttpPost("increase-quantity/{productId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [Authorize("UserPolicy")]
        public IActionResult IncreaseProductQuantity(int productId)
        {
            var username = User.Identity?.Name;
            int userId = _userRepository.GetUserIdByUsername(username);

            var increasedQuantity = _shoppingCartService.increaseProductQuantity(userId, productId);

            if (increasedQuantity)
            {
                return Ok("Product quantity increased successfully.");
            }
            else
            {
                ModelState.AddModelError("", "Unable to increase product quantity.");
                return BadRequest(ModelState);
            }
        }

        [HttpPost("decrease-quantity/{productId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [Authorize("UserPolicy")]
        public IActionResult DecreaseProductQuantity(int productId)
        {
            var username = User.Identity?.Name;
            int userId = _userRepository.GetUserIdByUsername(username);

            var increasedQuantity = _shoppingCartService.decreaseProductQuantity(userId, productId);

            if (increasedQuantity)
            {
                return Ok("Product quantity decreased successfully.");
            }
            else
            {
                ModelState.AddModelError("", "Unable to decrease product quantity.");
                return BadRequest(ModelState);
            }
        }

        [HttpPost("order/")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [Authorize("UserPolicy")]
        public IActionResult Order()
        {
            var username = User.Identity?.Name;
            int userId = _userRepository.GetUserIdByUsername(username);
            bool orderPlaced = _shoppingCartService.order(userId);

            if (orderPlaced)
            {
                return Ok("Order placed successfully.");
            }
            else
            {
                return NotFound("User not found or shopping cart is empty.");
            }
        }
    }
}