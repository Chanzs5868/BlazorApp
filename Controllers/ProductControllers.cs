using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BlazorApp.Models;
using BlazorApp.Services;

namespace BlazorApp.Controllers;

[ApiController]
[Route("api/[controller]")]
[IgnoreAntiforgeryToken]
public class ProductsController : ControllerBase
{
    private readonly ProductService _service;

    public ProductsController(ProductService service)
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult GetAll() => Ok(_service.GetAll());

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var product = _service.GetById(id);
        return product is null ? NotFound() : Ok(product);
    }

    [HttpPost]
    [Authorize]
    public IActionResult Create(Product product)
    {
        if (!IsAdmin()) return Forbid();
        var created = _service.Create(product);
        return Ok(created);
    }

    [HttpPut("{id}")]
    [Authorize]
    public IActionResult Update(int id, Product product)
    {
        if (!IsAdmin()) return Forbid();
        var updated = _service.Update(id, product);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public IActionResult Delete(int id)
    {
        if (!IsAdmin()) return Forbid();
        var success = _service.Delete(id);
        return success ? Ok(new { message = "Deleted." }) : NotFound();
    }

    private bool IsAdmin() =>
        User.Claims.FirstOrDefault(c => c.Type == "IsAdmin")?.Value == "True";
}