using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToZeptyDAL
{
    public class OrderDetail
    {
        public int OrderDetailId { get; set; }
        public int OrderId { get; set; }
        public string ProductName { get; set; }
        public virtual Order Order { get; set; }
        public decimal price { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a valid quantity")]
        public int Quantity { get; set; }

        public int OrderStatus { get; set; }
    }
}
