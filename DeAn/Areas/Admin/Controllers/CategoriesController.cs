using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DeAn.Models;

namespace DeAn.Areas.Admin.Controllers
{
    public class CategoriesController : Controller
    {
        private MyShopEntities db = new MyShopEntities();

        // GET: Admin/Categories
        public ActionResult Index()
        {
            return View(db.Categories.ToList());
        }

        // GET: Admin/Categories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // GET: Admin/Categories/Create
        //[HttpGet]  
        public ActionResult Create()
        {
            return View();
        }
        // POST: Admin/Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CategoryID,CategoryName")] Category category)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra nếu tên danh mục đã tồn tại trong cơ sở dữ liệu
                bool isCategoryExist = db.Categories.Any(c => c.CategoryName == category.CategoryName);
                if (isCategoryExist)
                {
                    // Thêm lỗi vào ModelState nếu tên danh mục đã tồn tại
                    ModelState.AddModelError("CategoryName", "Tên danh mục đã tồn tại.");
                    return View(category);
                }

                db.Categories.Add(category);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(category);
        }

        // GET: Admin/Categories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }
        // POST: Admin/Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CategoryID,CategoryName")] Category category)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra nếu tên danh mục mới đã tồn tại trong cơ sở dữ liệu, ngoại trừ danh mục hiện tại
                bool isCategoryExist = db.Categories.Any(c => c.CategoryName == category.CategoryName && c.CategoryID != category.CategoryID);
                if (isCategoryExist)
                {
                    // Thêm lỗi vào ModelState nếu tên danh mục đã tồn tại
                    ModelState.AddModelError("CategoryName", "Tên danh mục đã tồn tại.");
                    return View(category);
                }

                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(category);
        }

        // GET: Admin/Categories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }
        // POST: Admin/Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            // Tìm danh mục cần xóa
            Category category = db.Categories.Find(id);
            // Kiểm tra xem danh mục này có sản phẩm nào đang sử dụng không
            bool isCategoryInUse = db.Products.Any(p => p.CategoryID == id);
            if (isCategoryInUse)
            {
                // Nếu có sản phẩm sử dụng danh mục này, không cho phép xóa
                ModelState.AddModelError("", "Không thể xóa danh mục này vì nó đang được sử dụng trong một hoặc nhiều sản phẩm.");
                return View(category); // Trả về trang chi tiết danh mục với thông báo lỗi
            }

            // Nếu không có sản phẩm nào sử dụng, thực hiện xóa
            db.Categories.Remove(category);
            db.SaveChanges();

            // Quay lại trang danh sách danh mục
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
