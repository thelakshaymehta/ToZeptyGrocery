using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToZeptyUI.Models
{
    public class AddressViewModel
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public string Street { get; set; }
        public string HouseNo { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
    }
}