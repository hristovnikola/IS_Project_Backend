using AutoMapper;
using Domain;
using Domain.Dto;
using Microsoft.AspNetCore.Mvc;
using Service.Implementation;
using Service.Interface;


namespace Web.Controllers;


[Route("api/[controller]")]
[ApiController]
public class ProductsController : Controller
{
    private readonly IProductService _productService;
    private readonly IMapper _mapper;
    
    public ProductsController(IProductService productService,  IMapper mapper)
    {
        _productService = productService;
        _mapper = mapper;
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
    public IActionResult DeleteCountry(int productId)
    {
        if (!_productService.ProductExists(productId))
        {
            return NotFound();
        }

        _productService.DeleteProduct(productId);

        return NoContent();
    }
}