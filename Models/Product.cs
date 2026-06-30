namespace BlazorApp.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
    public decimal? OriginalPrice { get; set; }
    public string Category { get; set; } = "";
    public double Rating { get; set; }
    public int Reviews { get; set; }
    public string? Badge { get; set; }
}