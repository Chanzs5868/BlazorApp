using BlazorApp.Data;
using BlazorApp.Models;

namespace BlazorApp.Services;

public class ProductService
{
    private readonly AppDbContext _db;

    public ProductService(AppDbContext db)
    {
        _db = db;
    }

    public List<Product> GetAll() => _db.Products.ToList();

    public Product? GetById(int id) => _db.Products.FirstOrDefault(p => p.Id == id);

    public Product Create(Product product)
    {
        _db.Products.Add(product);
        _db.SaveChanges();
        return product;
    }

    public Product? Update(int id, Product updated)
    {
        var product = _db.Products.FirstOrDefault(p => p.Id == id);
        if (product is null) return null;

        product.Name = updated.Name;
        product.Price = updated.Price;
        product.OriginalPrice = updated.OriginalPrice;
        product.Category = updated.Category;
        product.Rating = updated.Rating;
        product.Reviews = updated.Reviews;
        product.Badge = updated.Badge;

        _db.SaveChanges();
        return product;
    }

    public bool Delete(int id)
    {
        var product = _db.Products.FirstOrDefault(p => p.Id == id);
        if (product is null) return false;

        _db.Products.Remove(product);
        _db.SaveChanges();
        return true;
    }
}