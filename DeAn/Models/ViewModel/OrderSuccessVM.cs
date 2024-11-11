using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DeAn.Models.ViewModel
{
    public class OrderSuccessVM
    {
        public int OrderID { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; }
        public string ShippingMethod { get; set; }
        public string ShippingAddress { get; set; }
        public List<OrderDetailVM> OrderDetails { get; set; }
    }

   
}