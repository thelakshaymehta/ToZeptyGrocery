using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToZeptyUI.Models
{
    public class OrderDetailsViewModel
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Subtotal { get; set; }
        public int ProductId { get; set; }
        public int OrderStatus { get; set; }
    }
}