using DeAn.Models;
using DeAn.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DeAn.Controllers
{
    public class OrderController : Controller
    {
        private MyShopEntities db = new MyShopEntities();
        private CartService GetCartService()
        {
            return new CartService(Session);
        }

        // GET: Order
        public ActionResult Index()
        {
            return View();
        }
        //Get: Order/Checkout
        [Authorize]
        public ActionResult Checkout()
        {
            //kiểm tra giỏ hàng trong section
            //nếu    hàng rỗng hoặc không có sản phẩm thì chuyển hướng về trang chủ

           
            //var carxt = Session["Cart"] as List<CartItem>;
            var cart = GetCartService().GetCart();
            if (cart.Items.Count() == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            //Xác thực ng dùng đã đăng nhập chưa, nếu chưa thì chuyển hướng tới đănh nhập
            string myUserName = Session["UserName"]?.ToString();
            var user = db.Users.SingleOrDefault(u =>  u.Username == myUserName);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //lấy thông tin từ khách hàng từ CSDL, nếu chưa thì chueyenr hướng tới trang đăng nhập
            //nếu rồi thì lấy địa chỉ của khách hànng và gáng vào ShippingAddress của checkoutVM
            var customer = db.Customers.SingleOrDefault(c => c.Username == user.Username);
            if (customer == null)
            {
                return RedirectToAction("Login", "Account");

            }
            var model = new CheckoutVM
            { //tạo dữ liệu hiển thị cho checkoutVm
                CartItems = cart.Items.ToList(),//Lấy danh sách các sản phẩm trong giỏ 
                TotalAmount = cart.Items.Sum(item => item.TotalPrice), // Tổng giá trị các mặt hàng trong giỏ
                OrderDate = DateTime.Now,//mặc định lấy bằng thời điểm đặt hàng
                ShippingAddress = customer.CustomerAddress,//Lấy địa chỉ mặc định từ bẳng customer
                CustomerID = customer.CustomerID,//lấy mã khách từ bảng Customer
                Username = customer.Username
            };
            return View(model);
        }
        //POST: Order/Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Checkout(CheckoutVM model)
        {
            if (ModelState.IsValid)
            {
                //var cart = Session["Cart"] as List<CartItem>;
                ////Nếu giỏ hàng rổng chueyern tới trang Home
                //if (cart == null || !cart.Any()) { return RedirectToAction("Index", "Home"); }
                ////Nếu ng dùng ch đăng nhập chuyển đén trang đăng nhập
                //var user = db.Users.SingleOrDefault(u => u.Username == User.Identity.Name);
                //if (user == null) { return RedirectToAction("Login", "Account"); }
                var cart = GetCartService().GetCart();
                if (cart.Items.Count() == 0)
                {
                    return RedirectToAction("Index", "Home");
                }
                //Xác thực ng dùng đã đăng nhập chưa, nếu chưa thì chuyển hướng tới đănh nhập
                string myUserName = Session["UserName"]?.ToString();
                var user = db.Users.SingleOrDefault(u => u.Username == myUserName);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                //nếu khách hàng k khớp với tên đn  sẽ chueyern đến trang đăng nhập(login)
                var customer = db.Customers.SingleOrDefault(c => c.Username == user.Username);
                if (customer == null) { return RedirectToAction("Login", "Account"); }
                //Nếu ng dùng chọn thanh toán bằn Paypal, sẽ chuyển đến trang PaymentWithPaypal
                if (model.PaymentMethod == "Paypal")
                {
                    return RedirectToAction(" PaymentWithPaypal", "Paypal", model);
                }
                //Thiết lập PaypalStatus dựa trên PaymentMethod
                string paymentStatus = "Chưa thanh toán";
                switch (model.PaymentMethod)
                {
                    case "Tiền mặt": paymentStatus = "Thanh toán tiền mặt"; break;
                    case "Paypal": paymentStatus = "Thanh toán paypal"; break;
                    case "Mua trước trả sau": paymentStatus = "Trả góp"; break;
                    default: paymentStatus = "Chưa thanh toán"; break;
                }
                //Tạo đơn hàng và chi tiết đơn hàng liên quan
                var order = new Order{
                    CustomerID = customer.CustomerID,
                    OrderDate = model.OrderDate,
                    TotalAmount = model.TotalAmount,
                    PaymentStatus = paymentStatus,
                    PaymentMethod = model.PaymentMethod,
                    ShippingMethod= model.ShippingMethod,
                    ShippingAddress = model.ShippingAddress,
                    OrderDetails = cart.Items.Select(item => new OrderDetail
                    {
                        ProductID = item.ProductID,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        TotalPrice = item.TotalPrice
                    }).ToList()
                };
                //Lưu đơn hàng vào CSDL 
                db.Orders.Add(order);
                db.SaveChanges();
                //Xóa giỏ hàng sau khi đặt hàng thành công  
                Session["Cart"] = null;
                //Chuyển đến trang xác nhận đơn hàng
                return RedirectToAction("OrderSuccess", new { id = order.OrderID });
            }
            return View(model);
        }
        // GET: Order/OrderSuccess
        [Authorize]
        public ActionResult OrderSuccess(int id)
        {
            // Lấy thông tin đơn hàng từ CSDL dựa trên OrderID
            var order = db.Orders.SingleOrDefault(o => o.OrderID == id);

            // Kiểm tra nếu đơn hàng không tồn tại
            if (order == null)
            {
                return HttpNotFound();
            }

            // Tạo một model để truyền dữ liệu sang View
            var model = new OrderSuccessVM
            {
                OrderID = order.OrderID,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                PaymentMethod = order.PaymentMethod,
                //PaymentStatus = order.PaymentStatus,
                ShippingMethod = order.ShippingMethod,
                ShippingAddress = order.ShippingAddress,
                OrderDetails = order.OrderDetails.Select(od => new OrderDetailVM
                {
                    ProductName = od.Product.ProductName, // Giả sử bạn có thuộc tính Name trong bảng Product
                    Quantity = od.Quantity,
                    UnitPrice = od.UnitPrice,
                    TotalPrice = od.TotalPrice
                }).ToList()
            };

            return View(model);
        }
        [Authorize]
        public ActionResult MyOrders()
        {
            // Lấy tên người dùng từ session
            string myUserName = Session["UserName"]?.ToString();

            if (string.IsNullOrEmpty(myUserName))
            {
                return RedirectToAction("Login", "Account");
            }

            // Lấy thông tin người dùng từ cơ sở dữ liệu
            var user = db.Users.SingleOrDefault(u => u.Username == myUserName);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Lấy tất cả đơn hàng của người dùng này
            var orders = db.Orders
                           .Where(o => o.Customer.Username == user.Username)
                           .OrderByDescending(o => o.OrderDate) // Sắp xếp theo ngày đặt hàng mới nhất
                           .ToList();

            // Chuyển đổi dữ liệu đơn hàng thành một danh sách ViewModel
            var model = orders.Select(o => new MyOrdersVM
            {
                OrderID = o.OrderID,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                PaymentStatus = o.PaymentStatus,
                PaymentMethod = o.PaymentMethod,
                ShippingMethod = o.ShippingMethod,
                ShippingAddress = o.ShippingAddress,
                OrderStatus = GetOrderStatus(o) // Gọi hàm để lấy trạng thái đơn hàng
            }).ToList();

            return View(model);
        }
        private string GetOrderStatus(Order order)
        {
            // Dựa vào PaymentStatus và các trạng thái khác để xác định OrderStatus
            if (order.PaymentStatus == "Chưa thanh toán")
            {
                return "Chờ thanh toán"; // Trạng thái khi chưa thanh toán
            }
            if (order.PaymentStatus == "Thanh toán tiền mặt" && order.ShippingMethod != null)
            {
                return "Chờ xử lý"; // Trạng thái khi đã thanh toán nhưng đơn hàng vẫn đang chờ xử lý
            }
            if (order.PaymentStatus == "Thanh toán paypal")
            {
                return "Đã thanh toán"; // Trạng thái khi đã thanh toán qua Paypal
            }

            return "Chưa xác định"; // Trạng thái mặc định nếu không khớp với các điều kiện trên
        }

    }
}