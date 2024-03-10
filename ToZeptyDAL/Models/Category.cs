﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToZeptyDAL
{
    public class Category
    {
        public int CategoryId { get; set; }

        [MaxLength(255)]
        public string CategoryName { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}
