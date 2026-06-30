using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BlazorApp.Services;

namespace BlazorApp.Controllers;

[ApiController]
[Route("api/orders")]
[Authorize]
[IgnoreAntiforgeryToken]
public class OrdersController : ControllerBase
{
    private readonly OrderService _orderService;

    public OrdersController(OrderService orderService)
    {
        _orderService = orderService;
    }

    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost]
    public IActionResult PlaceOrder()
    {
        var order = _orderService.PlaceOrder(GetUserId());
        if (order is null)
            return BadRequest(new { message = "Cart is empty." });

        return Ok(order);
    }

    [HttpGet]
    public IActionResult GetOrders()
    {
        var orders = _orderService.GetOrders(GetUserId());
        return Ok(orders);
    }

    [HttpGet("all")]
    public IActionResult GetAllOrders()
    {
        var isAdmin = User.Claims.FirstOrDefault(c => c.Type == "IsAdmin")?.Value == "True";
        if (!isAdmin) return Forbid();

        var orders = _orderService.GetAllOrders();
        return Ok(orders);
    }

    [HttpGet("{id}")]
    public IActionResult GetOrder(int id)
    {
        var order = _orderService.GetOrder(GetUserId(), id);
        return order is null ? NotFound() : Ok(order);
    }
}