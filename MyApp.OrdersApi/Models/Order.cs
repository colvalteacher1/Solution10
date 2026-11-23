using System.ComponentModel.DataAnnotations;

namespace MyApp.OrdersApi.Models;

public class Order
{
    public int Id { get; set; }

    [Range(1, int.MaxValue)]
    public int ProductId { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    public decimal TotalPrice { get; set; }
}
