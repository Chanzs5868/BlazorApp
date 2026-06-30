using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BlazorApp.Models;
using BlazorApp.Services;

namespace BlazorApp.Controllers;

[ApiController]
[Route("api/cart")]
[Authorize]
[IgnoreAntiforgeryToken]
public class CartController : ControllerBase
{
    private readonly CartService _cartService;

    public CartController(CartService cartService)
    {
        _cartService = cartService;
    }

    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public IActionResult GetCart()
    {
        var items = _cartService.GetCart(GetUserId());
        return Ok(items);
    }

    [HttpPost]
    public IActionResult AddToCart(AddToCartRequest request)
    {
        _cartService.AddToCart(GetUserId(), request.ProductId, request.Quantity);
        return Ok(new { message = "Added to cart." });
    }

    [HttpDelete("{productId}")]
    public IActionResult RemoveFromCart(int productId)
    {
        _cartService.RemoveFromCart(GetUserId(), productId);
        return Ok(new { message = "Removed from cart." });
    }
    
    [HttpPut("{productId}")]
    public IActionResult UpdateQuantity(int productId, [FromBody] UpdateQuantityRequest request)
    {
        _cartService.UpdateQuantity(GetUserId(), productId, request.Quantity);
        return Ok(new { message = "Cart updated." });
    }
}