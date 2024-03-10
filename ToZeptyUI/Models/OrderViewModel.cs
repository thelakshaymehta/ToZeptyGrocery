using ToZeptyDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToZeptyUI.Models
{
    public class OrderViewModel
    {
        public string name {  get; set; }
        public int CustId { get; set; }
        public string UserName { get; set; }
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderDetailsViewModel> OrderDetails { get; set; }
        public List<AddressViewModel> Addresses { get; set; }
        public int orderStatus { get; set; }
    }
}