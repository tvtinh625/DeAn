using DeAn.Models;
using DeAn.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DeAn.Controllers
{
    public class CartController : Controller
    {
        private MyShopEntities db = new MyShopEntities();
        private CartService GetCartService()
        {
            return new CartService(Session);
        }
        //Hiển thị giỏ hàng k gom nhóm theo dm
        public ActionResult Index()
        {
            var cart = GetCartService().GetCart();
            return View(cart);
        }
        //thêm sp vào giỏ hàng
        public ActionResult AddToCart(int id, int quantity =1)
        {
            var product = db.Products.Find(id);
            if(product != null)
            {
                var cartService = GetCartService();
                cartService.GetCart().AddItem(product.ProductID, product.ProductImage, product.ProductName,
                    product.ProductPrice, quantity, product.Category.CategoryName);
            }
            return RedirectToAction("Index");
                    
        }
 
        //Xóa
        public ActionResult RemoveFromCart(int id)
        {
            var cartService = GetCartService();
            cartService.GetCart().RemoveItem(id);
            return RedirectToAction("Index");
        }
        public ActionResult ClearCart()
        {
            GetCartService().ClearCart();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult UpdateQuantity(int id, int quantity)
        {
            var cartService = GetCartService();
            cartService.GetCart().UpdateQuantity(id, quantity);
            return RedirectToAction("Index");
        }
    }
}