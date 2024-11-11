using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeAn.Models.ViewModel
{
 
        public class MyOrdersVM
        {
            public int OrderID { get; set; } // Mã đơn hàng
            public DateTime OrderDate { get; set; } // Ngày đặt hàng
            public decimal TotalAmount { get; set; } // Tổng số tiền
            public string PaymentStatus { get; set; } // Trạng thái thanh toán
            public string PaymentMethod { get; set; } // Phương thức thanh toán
            public string ShippingMethod { get; set; } // Phương thức giao hàng
            public string ShippingAddress { get; set; } // Địa chỉ giao hàng
            public string OrderStatus { get; set; }
    }

    
}