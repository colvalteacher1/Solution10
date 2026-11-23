using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApp.OrdersApi.Data;
using MyApp.OrdersApi.Models;

namespace MyApp.OrdersApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrdersController(OrdersDbContext dbContext, IHttpClientFactory httpClientFactory) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Order>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await dbContext.Orders.AsNoTracking().ToListAsync(cancellationToken));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Order>> GetOne(int id, CancellationToken cancellationToken)
    {
        var order = await dbContext.Orders
            .AsNoTracking()
            .FirstOrDefaultAsync(order => order.Id == id, cancellationToken);

        return order is null
            ? NotFound(new { message = "Order not found" })
            : Ok(order);
    }

    [HttpPost]
    public async Task<ActionResult<Order>> Create(Order order, CancellationToken cancellationToken)
    {
        var productsClient = httpClientFactory.CreateClient("ProductsService");

        try
        {
            using var response = await productsClient.GetAsync(
                $"api/products/{order.ProductId}",
                cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return NotFound(new { message = "Product not found" });
            }

            response.EnsureSuccessStatusCode();

            var product = await response.Content.ReadFromJsonAsync<ProductResponse>(cancellationToken);
            if (product is null)
            {
                return StatusCode(StatusCodes.Status502BadGateway, new
                {
                    message = "ProductsService returned an invalid response."
                });
            }

            order.TotalPrice = order.Quantity * product.Price;
            dbContext.Orders.Add(order);
            await dbContext.SaveChangesAsync(cancellationToken);

            return CreatedAtAction(nameof(GetOne), new { id = order.Id }, order);
        }
        catch (HttpRequestException)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new
            {
                message = "ProductsService is unavailable."
            });
        }
        catch (JsonException)
        {
            return StatusCode(StatusCodes.Status502BadGateway, new
            {
                message = "ProductsService returned an invalid response."
            });
        }
    }

    private sealed class ProductResponse
    {
        public decimal Price { get; set; }
    }
}