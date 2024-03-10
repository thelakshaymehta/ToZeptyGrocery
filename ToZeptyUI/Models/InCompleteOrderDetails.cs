using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToZeptyUI.Models
{
    public class InCompleteOrderDetails
    {
    
        public int CustomerId { get; set; }
        public int OrderId { get; set; }
        public int OrderDetails { get; set; }
        public int OrderStatus { get; set; }

    }
}