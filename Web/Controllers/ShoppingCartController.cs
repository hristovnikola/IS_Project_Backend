using Microsoft.AspNetCore.Mvc;
using Domain.Dto; // Import your DTOs
using Service.Interface; // Import your service interface

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IShoppingCartService _shoppingCartService;

        public ShoppingCartController(IProductService productService, IShoppingCartService shoppingCartService)
        {
            _productService = productService;
            _shoppingCartService = shoppingCartService;
        }

        [HttpGet("shopping-cart-info/{userId}")]
        [ProducesResponseType(200, Type = typeof(ShoppingCartDto))] // Adjust the response type to match your DTO
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetShoppingCartInfo(int userId)
        {
            var shoppingCartInfo = _shoppingCartService.getShoppingCartInfo(userId);

            if (shoppingCartInfo == null)
            {
                return NotFound();
            }

            return Ok(shoppingCartInfo);
        }

        [HttpDelete("remove-product/{userId}/{productId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult RemoveProductFromShoppingCart(int userId, int productId)
        {
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
        
        [HttpPost("order/{userId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult Order(int userId)
        {
            try
            {
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
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}