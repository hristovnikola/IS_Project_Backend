using System.Security.Claims;
using AutoMapper;
using Domain;
using Domain.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository.Interface;
using Service.Implementation;
using Service.Interface;


namespace Web.Controllers;


[Route("api/[controller]")]
[ApiController]
public class ProductsController : Controller
{
    private readonly IProductService _productService;
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;
    
    public ProductsController(IProductService productService,  IMapper mapper, IUserRepository userRepository)
    {
        _productService = productService;
        _mapper = mapper;
        _userRepository = userRepository;
    }
    
    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Product>))]
    public IActionResult GetAllProducts()
    {
        var products = _mapper.Map<List<ProductDto>>(_productService.GetAllProducts());
        
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(products);
    }
    
    [HttpGet("{productId}")]
    [ProducesResponseType(200, Type = typeof(Product))]
    [ProducesResponseType(400)]
    public IActionResult GetProduct(int productId)
    {
        if (!_productService.ProductExists(productId))
            return NotFound();

        var product = _mapper.Map<ProductDto>(_productService.GetProduct(productId));

        if (!ModelState.IsValid)
            return BadRequest();

        return Ok(product);
    }
    
    [HttpPost]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [Authorize("AdminPolicy")]
    public IActionResult CreateProduct([FromBody] ProductDto productCreate)
    {
        if (productCreate == null)
            return BadRequest(ModelState);

        var product = _productService.GetAllProducts()
            .Where(c => c.Name.Trim().ToUpper()== productCreate.Name.TrimEnd().ToUpper()) 
            .FirstOrDefault();

        if (product != null)
        {
            ModelState.AddModelError("", "Product already exists");
            return StatusCode(422, ModelState);
        }

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var productMap = _mapper.Map<Product>(productCreate);

        if (!_productService.CreateProduct(productMap))
        {
            ModelState.AddModelError("", "Something went wrong while saving");
            return StatusCode(500, ModelState);
        }

        return Ok("Successfully created");
    }
    
    [HttpPut]
    [ProducesResponseType(400)]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [Authorize("AdminPolicy")]
    public IActionResult UpdateProduct([FromBody] ProductDto updatedProduct)
    {
        if (updatedProduct == null)
        {
            return BadRequest(ModelState);
        }

        // var product = _productService.GetProduct(updatedProduct.Id);
        //
        // if (product == null)
        // {
        //     return BadRequest(ModelState);
        // }

        if (!_productService.ProductExists(updatedProduct.Id))
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        // _productService.AttachProduct(productMap);
        // productMap.Id = updatedProduct.Id;
        
        var productMap = _mapper.Map<Product>(updatedProduct);

        if (!_productService.UpdateProduct(productMap))
        {
            ModelState.AddModelError("", "Something went wrong while updating product");
            return StatusCode(500, ModelState);
        }

        return NoContent();
    }
    
    [HttpDelete("{productId}")]
    [ProducesResponseType(400)]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [Authorize("AdminPolicy")]
    public IActionResult DeleteProduct(int productId)
    {
        if (!_productService.ProductExists(productId))
        {
            return NotFound();
        }

        _productService.DeleteProduct(productId);

        return NoContent();
    }
    
    [HttpGet("shopping-cart-info/{productId}")]
    [ProducesResponseType(200, Type = typeof(AddToShoppingCardDto))]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public IActionResult GetShoppingCartInfo(int productId)
    {
        var shoppingCartInfo = _productService.GetShoppingCartInfo(productId);
    
        if (shoppingCartInfo == null)
            return NotFound();

        return Ok(shoppingCartInfo);
    }
    
    [HttpPost("add-to-shopping-cart")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public IActionResult AddToShoppingCart([FromBody] AddToShoppingCardDto item)
    {
        if (item == null)
            return BadRequest(ModelState);

        var username = User.Identity?.Name;
        int userId = _userRepository.GetUserIdByUsername(username);

        bool addedToCart = _productService.AddToShoppingCart(userId, item);

        
        if (addedToCart)
        {
            return Ok("Product added to the shopping cart successfully.");
        }
        else
        {
            ModelState.AddModelError("", "Unable to add the product to the shopping cart.");
            return BadRequest(ModelState);
        }
    }

}