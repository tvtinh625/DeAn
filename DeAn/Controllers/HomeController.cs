using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DeAn.Models;
using DeAn.Models.ViewModel;
using PagedList;

namespace DeAn.Controllers
{
    public class HomeController : Controller
    {
        private MyShopEntities db = new MyShopEntities();
        public ActionResult Index(string searchTerm, int? page)
        {
            var model = new HomeProductVM();
            var products = db.Products.AsQueryable();
            if (!string.IsNullOrEmpty(searchTerm))
            {
                model.SearchTerm = searchTerm;
                products = products.Where(p => p.ProductName.Contains(searchTerm) ||
                p.ProductDescription.Contains(searchTerm) ||
                p.Category.CategoryName.Contains(searchTerm));
            }
            int pageNumber = page ?? 1;
            int pageSize = 6;
            model.FeaturedProducts = products.OrderByDescending(p => p.OrderDetails.Count()).Take(10).ToList();

            model.NewProducts = products.OrderBy(p => p.OrderDetails.Count()).Take(20).ToPagedList(pageNumber, pageSize);
            return View(model);
        }

        public ActionResult ProductDetails(int? id, int? quantity, int? page)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product pro = db.Products.Find(id);
            if (pro == null)
            {
                return HttpNotFound();
            }
            var products = db.Products.Where(p => p.CategoryID == pro.CategoryID && p.ProductID != pro.ProductID).AsQueryable();
            ProductDetailsVM model = new ProductDetailsVM();
            int pageNumber = page ?? 1;
            int pageSize = model.PageSize;
            model.product = pro;
            model.RelatedProducts = products.OrderBy(p => p.ProductID).Take(8).ToPagedList(pageNumber, pageSize);
            model.TopProducts = products.OrderByDescending(p => p.OrderDetails.Count()).Take(8).ToPagedList(pageNumber, pageSize);
            if (quantity.HasValue)
            {
                model.quantity = quantity.Value;
            }
            return View(model);
        }


        public ActionResult Product(int? page, string searchTerm)
        {
            int pageSize = 6;
            int pageNumber = (page ?? 1);

            var products = db.Products.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                products = products.Where(p => p.ProductName.Contains(searchTerm));
            }

            var model = products.OrderBy(p => p.ProductName)
                                .ToPagedList(pageNumber, pageSize);

            var homeVM = new HomeProductVM();
            homeVM.PageNumber = pageNumber;
            homeVM.NewProducts = model;

            return View(homeVM);
        }
        public ActionResult Checkout()
        {
            return View();
        }
      public ActionResult introduce()
        {
            return View();
        }
    }
}
