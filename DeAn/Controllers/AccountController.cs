using DeAn.Models;
using DeAn.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace DeAn.Controllers
{
    public class AccountController : Controller
    {
        private MyShopEntities db = new MyShopEntities();
        // GET: Account
        public ActionResult Register()
        {
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterVM model)
        {
            if (ModelState.IsValid)
            {
                // kiem tra ten dang nhap
                var existingUser = db.Users.SingleOrDefault(u => u.Username == model.Username);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Username", "Tên đăng nhập này đã tồn tại!");
                    return View(model);
                }

                // neu chua ton tai thi tao ban ghi thong tin tk trong bang user
                var user = new User
                {
                    Username = model.Username,
                    Password = model.Password,
                    UserRole = "C"

                };
                db.Users.Add(user);
                // va tao ban ghi thong tin khach hang trong bang customer
                var customer = new Customer
                {
                    CustomerName = model.CustomerName,
                    CustomerEmail = model.CustomerEmail,
                    CustomerPhone = model.CustomerPhone,
                    CustomerAddress = model.CustomerAddress,
                    Username = model.Username,
                };
                db.Customers.Add(customer);
                // luu thong tin tai khoan vao csdl
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }
        // Get: Account/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginVM model)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.SingleOrDefault(u => u.Username == model.UserName
                && u.Password == model.Password
                && u.UserRole == "C");
                if (user != null)
                {
                    //Lưu trạng thái đăng nhập vào session
                    Session["UserName"] = user.Username;
                    Session["UserRole"] = user.UserRole;

                    // lưu thông tin xác thực người dùng cookie
                    FormsAuthentication.SetAuthCookie(user.Username, false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng");
                }
            }
            return View();
        }

        public ActionResult Logout()
        {
            // Đăng xuất người dùng
            FormsAuthentication.SignOut(); // Nếu bạn dùng Forms Authentication
            Session.Clear(); // Xóa dữ liệu session (nếu sử dụng session)
            Session.Abandon(); // Xóa session hiện tại
            TempData["Message"] = "Đã đăng xuất thành công"; // Thông báo đăng xuất thành công

            // Chuyển hướng về trang đăng nhập hoặc trang chủ
            return RedirectToAction("Login", "Account");
        }
    }
}
