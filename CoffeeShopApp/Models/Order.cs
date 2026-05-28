using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeShopApp.Models;

public class Order
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "Виконано";
    public string PaymentMethod { get; set; } = "Готівка";

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}
