using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DeAn.Models;
using DeAn.Models.ViewModel;
using PagedList;

namespace DeAn.Areas.Admin.Controllers
{
    public class ProductsController : Controller
    {
        private MyShopEntities db = new MyShopEntities();

        // GET: Admin/Products
        //public ActionResult Index(string SearchTerm, decimal? minPrice, decimal? maxPrice, string sortOrder, int? page)
        //{
        //    var model = new ProductSearchVM();
        //    var products = db.Products.AsQueryable();
        //    if (!string.IsNullOrEmpty(SearchTerm))
        //    {
        //        products = products.Where(p =>
        //        p.ProductName.Contains(SearchTerm) ||
        //        p.ProductDescription.Contains(SearchTerm) ||
        //        p.Category.CategoryName.Contains(SearchTerm));
        //    }
        //    if (minPrice.HasValue)
        //    {
        //        products = products.Where(p => p.ProductPrice >= minPrice.Value);
        //    }
        //    if (maxPrice.HasValue)
        //    {
        //        products = products.Where(p => p.ProductPrice <= maxPrice.Value);
        //    }
        //    switch (sortOrder)
        //    {
        //        case "name_asc":
        //            products = products.OrderBy(p => p.ProductName);
        //            break;
        //        case "name_desc":
        //            products = products.OrderByDescending(p => p.ProductName);
        //            break;
        //        case "price_asc":
        //            products = products.OrderBy(p => p.ProductPrice);
        //            break;
        //        case "price_desc":
        //            products = products.OrderByDescending(p => p.ProductPrice);
        //            break;
        //        default:
        //            products = products.OrderBy(p => p.ProductName);
        //            break;
        //    }
        //    model.sortOrder = sortOrder;
        //    int pageNumber = page ?? 1;
        //    int pageSize = 2;
        //    model.Products = products.ToPagedList(pageNumber, pageSize);
        //    return View(model);
        //}
        public ActionResult Index(string SearchTerm, decimal? minPrice, decimal? maxPrice, string sortOrder, int? page)
        {
            var model = new ProductSearchVM();
            var products = db.Products.AsQueryable();

            if (!string.IsNullOrEmpty(SearchTerm))
            {
                products = products.Where(p =>
                    p.ProductName.Contains(SearchTerm) ||
                    p.ProductDescription.Contains(SearchTerm) ||
                    p.Category.CategoryName.Contains(SearchTerm));
            }

            if (minPrice.HasValue)
            {
                products = products.Where(p => p.ProductPrice >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                products = products.Where(p => p.ProductPrice <= maxPrice.Value);
            }

            // Sorting logic
            switch (sortOrder)
            {
                case "name_asc":
                    products = products.OrderBy(p => p.ProductName);
                    break;
                case "name_desc":
                    products = products.OrderByDescending(p => p.ProductName);
                    break;
                case "price_asc":
                    products = products.OrderBy(p => p.ProductPrice);
                    break;
                case "price_desc":
                    products = products.OrderByDescending(p => p.ProductPrice);
                    break;
                default:
                    products = products.OrderBy(p => p.ProductName);
                    break;
            }

            // Pagination
            int pageNumber = page ?? 1;
            int pageSize = 2;  // Adjust page size as needed
            model.Products = products.ToPagedList(pageNumber, pageSize);

            model.sortOrder = sortOrder;

            return View(model);
        }


        // GET: Admin/Products/Create
        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName");
            return View();
        }

        // POST: Admin/Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "ProductID,CategoryID,ProductName,ProductDescription,ProductPrice,ProductImage")] Product product)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Products.Add(product);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
        //    return View(product);
        //}
        // POST: Admin/Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductID,CategoryID,ProductName,ProductDescription,ProductPrice,ProductImage")] Product product)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra xem sản phẩm có tên trùng với sản phẩm đã tồn tại trong cơ sở dữ liệu không
                bool isProductExist = db.Products.Any(p => p.ProductName == product.ProductName);
                if (isProductExist)
                {
                    // Nếu có sản phẩm trùng tên, thêm lỗi vào ModelState để hiển thị thông báo lỗi
                    ModelState.AddModelError("ProductName", "Tên sản phẩm đã tồn tại.");
                    ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
                    return View(product);
                }

                // Nếu không trùng tên, thêm sản phẩm vào cơ sở dữ liệu
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            // Nếu có lỗi, hiển thị lại danh sách danh mục
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        //Detail
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }
        // GET: Admin/Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // POST: Admin/Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductID,CategoryID,ProductName,ProductDescription,ProductPrice,ProductImage")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // GET: Admin/Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Admin/Products/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Product product = db.Products.Find(id);
        //    db.Products.Remove(product);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}
        // POST: Admin/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);

            // Kiểm tra xem sản phẩm có tồn tại trong bất kỳ đơn hàng nào không
            bool isProductInOrders = db.OrderDetails.Any(od => od.ProductID == id);

            if (isProductInOrders)
            {
                // Nếu sản phẩm đang tồn tại trong đơn hàng, không cho phép xóa và thông báo lỗi
                ModelState.AddModelError("", "Không thể xóa sản phẩm vì sản phẩm này đã được sử dụng trong đơn hàng.");
                return View(product); // Trả về lại trang Delete với thông báo lỗi
            }

            if (product == null)
            {
                return HttpNotFound();
            }

            // Nếu không có ràng buộc, thực hiện xóa sản phẩm
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
