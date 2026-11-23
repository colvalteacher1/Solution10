using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using MyApp.ProductsApi.Data;
using MyApp.ProductsApi.Models;

namespace MyApp.ProductsApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController(ProductsDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await dbContext.Products.AsNoTracking().ToListAsync(cancellationToken));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Product>> GetOne(int id, CancellationToken cancellationToken)
    {
        var product = await dbContext.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(product => product.Id == id, cancellationToken);

        return product is null
            ? NotFound(new { message = "Product not found" })
            : Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<Product>> Create(Product product, CancellationToken cancellationToken)
    {
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetOne), new { id = product.Id }, product);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Product product, CancellationToken cancellationToken)
    {
        if (id != product.Id)
        {
            return BadRequest(new { message = "The route id and product id must match." });
        }

        var existingProduct = await dbContext.Products.FindAsync([id], cancellationToken);
        if (existingProduct is null)
        {
            return NotFound(new { message = "Product not found" });
        }

        existingProduct.Name = product.Name;
        existingProduct.Price = product.Price;
        await dbContext.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var product = await dbContext.Products.FindAsync([id], cancellationToken);
        if (product is null)
        {
            return NotFound(new { message = "Product not found" });
        }

        dbContext.Products.Remove(product);
        await dbContext.SaveChangesAsync(cancellationToken);

        return NoContent();
    }
}
