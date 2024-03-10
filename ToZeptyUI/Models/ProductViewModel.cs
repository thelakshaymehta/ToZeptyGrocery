using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ToZeptyUI.Models
{
    public class ProductViewModel
    {
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Enter Product Id:")]
        public int ProductQuantity { get; set; }

        [Required(ErrorMessage = "Product Name is required:")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required:")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Price is required:")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Please enter a positive price")]
        public decimal Price { get; set; }

        [Display(Name = "Product Image:")]
        public HttpPostedFileBase ImageFile { get; set; }

        public string ImageFileName { get; set; }

        [Required(ErrorMessage = "Category is required:")]
        [Display(Name = "Product Category:")]
        public int CategoryId { get; set; }

        [Display(Name = "Category Name")]
        public string CategoryName { get; set; }
        public IEnumerable<SelectListItem> Categories { get; set; }
    }
}
