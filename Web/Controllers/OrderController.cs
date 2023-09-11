using AutoMapper;
using Domain;
using Domain.Dto;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;

namespace Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrderController : Controller
{
    private readonly IOrderService _orderService;
    private readonly IMapper _mapper;

    public OrderController(IOrderService orderService, IMapper mapper)
    {
        _orderService = orderService;
        _mapper = mapper;
    }
    
    [HttpGet("getAllOrders/")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Order>))]
    public IActionResult GetOrders()
    {
        var result = _mapper.Map<List<OrderDto>>(this._orderService.getAllOrders());
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(result);
    }
}