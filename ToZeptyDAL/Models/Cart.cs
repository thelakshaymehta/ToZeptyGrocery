using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToZeptyDAL
{
    public class Cart
    {
        [Key] 
        public int CartId { get; set; }

        [Required(ErrorMessage = "Customer is required.")]
        public int CusomerId { get; set; }
        public virtual  Customer  Customer{ get; set; }

        [Required(ErrorMessage = "Product name is required.")]
        [MaxLength(255)]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }
        public string ImageFileName { get; set; }
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
    }

}
