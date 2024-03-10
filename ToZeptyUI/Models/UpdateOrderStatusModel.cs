using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToZeptyUI.Models
{
    public class UpdateOrderStatusModel
    {
    public   string ProductName { get; set; }
    public int Quantity { get; set; }
    public int OrderId { get; set; }
    }
}