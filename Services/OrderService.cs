using BlazorApp.Data;
using BlazorApp.Models;

namespace BlazorApp.Services;

public class OrderService
{
    private readonly AppDbContext _db;

    public OrderService(AppDbContext db)
    {
        _db = db;
    }

    public OrderResponse? PlaceOrder(int userId)
    {
        var cartItems = _db.CartItems.Where(c => c.UserId == userId).ToList();
        if (cartItems.Count == 0) return null;

        var order = new Order { UserId = userId };

        foreach (var cartItem in cartItems)
        {
            var product = _db.Products.FirstOrDefault(p => p.Id == cartItem.ProductId);
            if (product is null) continue;

            order.Items.Add(new OrderItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                Price = product.Price,
                Quantity = cartItem.Quantity
            });
        }

        order.Total = order.Items.Sum(i => i.Price * i.Quantity);

        _db.Orders.Add(order);
        _db.CartItems.RemoveRange(cartItems);
        _db.SaveChanges();

        return ToResponse(order);
    }

    public OrderResponse? GetOrder(int userId, int orderId)
    {
        var order = _db.Orders.FirstOrDefault(o => o.Id == orderId && o.UserId == userId);
        if (order is null) return null;

        order.Items = _db.OrderItems.Where(i => i.OrderId == order.Id).ToList();
        return ToResponse(order);
    }
    public List<OrderResponse> GetOrders(int userId)
    {
        var orders = _db.Orders
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .ToList();

        foreach (var order in orders)
        {
            order.Items = _db.OrderItems.Where(i => i.OrderId == order.Id).ToList();
        }

        return orders.Select(ToResponse).ToList();
    }
    public List<OrderResponse> GetAllOrders()
    {
        var orders = _db.Orders
            .OrderByDescending(o => o.CreatedAt)
            .ToList();

        foreach (var order in orders)
        {
            order.Items = _db.OrderItems.Where(i => i.OrderId == order.Id).ToList();
        }

        return orders.Select(ToResponse).ToList();
    }
    private static OrderResponse ToResponse(Order order) => new()
    {
        Id = order.Id,
        Total = order.Total,
        CreatedAt = order.CreatedAt,
        Items = order.Items.Select(i => new OrderItemResponse
        {
            ProductId = i.ProductId,
            ProductName = i.ProductName,
            Price = i.Price,
            Quantity = i.Quantity
        }).ToList()
    };
}