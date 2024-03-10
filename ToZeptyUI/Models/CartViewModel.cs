using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ToZeptyUI.Models
{
    public class CartViewModel
    {
        public int CartId { get; set; }

        [Required(ErrorMessage = "Customer is required.")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Product name is required.")]
        [MaxLength(255)]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }
        public string ImageFileName { get; set; }
        public int ProductId { get; set; }
    }
}