using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeAn.Models.ViewModel
{
    public class OrderDetailVM
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal? TotalPrice { get; set; }
    }
}