using BlazorApp.Data;
using BlazorApp.Models;

namespace BlazorApp.Services;

public class CartService
{
    private readonly AppDbContext _db;

    public CartService(AppDbContext db)
    {
        _db = db;
    }

    public List<CartItemResponse> GetCart(int userId)
    {
        return _db.CartItems
            .Where(c => c.UserId == userId)
            .Join(_db.Products,
                c => c.ProductId,
                p => p.Id,
                (c, p) => new CartItemResponse
                {
                    ProductId = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Quantity = c.Quantity
                })
            .ToList();
    }

    public void AddToCart(int userId, int productId, int quantity)
    {
        var existing = _db.CartItems.FirstOrDefault(c => c.UserId == userId && c.ProductId == productId);
        if (existing is not null)
            existing.Quantity += quantity;
        else
            _db.CartItems.Add(new CartItem { UserId = userId, ProductId = productId, Quantity = quantity });

        _db.SaveChanges();
    }

    public void RemoveFromCart(int userId, int productId)
    {
        var item = _db.CartItems.FirstOrDefault(c => c.UserId == userId && c.ProductId == productId);
        if (item is not null)
        {
            _db.CartItems.Remove(item);
            _db.SaveChanges();
        }
    }

    public void UpdateQuantity(int userId, int productId, int quantity)
    {
        var item = _db.CartItems.FirstOrDefault(c => c.UserId == userId && c.ProductId == productId);
        if (item is null) return;

        if (quantity <= 0)
            _db.CartItems.Remove(item);
        else
            item.Quantity = quantity;

        _db.SaveChanges();
    }
}