
using Microsoft.AspNetCore.Mvc;

namespace MyApp.OrdersApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        // Liste des commandes en mémoire
        private static readonly List<Order> orders = new()
        {
            new Order { Id = 1, CustomerName = "Fred", ProductId = 1, Quantity = 2, Total = 2400 },
            new Order { Id = 2, CustomerName = "David", ProductId = 2, Quantity = 1, Total = 800 },
            new Order { Id = 3, CustomerName = "Samy", ProductId = 3, Quantity = 3, Total = 1500 },
            new Order { Id = 4, CustomerName = "Maya", ProductId = 4, Quantity = 2, Total = 300 }
        };

        // GET: api/orders
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(orders);
        }

        // GET: api/orders/1
        [HttpGet("{id}")]
        public IActionResult GetOne(int id)
        {
            var order = orders.FirstOrDefault(o => o.Id == id);

            if (order == null)
                return NotFound(new { message = "Order not found" });

            return Ok(order);
        }

        // POST: api/orders
        [HttpPost]
        public IActionResult Create(Order newOrder)
        {
            // Générer un nouvel identifiant
            newOrder.Id = orders.Max(o => o.Id) + 1;

            // Ajouter la commande
            orders.Add(newOrder);

            return CreatedAtAction(
                nameof(GetOne),
                new { id = newOrder.Id },
                newOrder
            );
        }

        // Classe Order
        public class Order
        {
            public int Id { get; set; }
            public string CustomerName { get; set; } = "";
            public int ProductId { get; set; }
            public int Quantity { get; set; }
            public decimal Total { get; set; }
        }
    }
}
