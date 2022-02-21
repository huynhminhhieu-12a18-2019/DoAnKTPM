using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DoAnKTPM.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using DoAnKTPM.Models;

namespace DoAnKTPM.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly DoAnKTPMContext _context;

        public HomeController(DoAnKTPMContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            if (HttpContext.Request.Cookies.ContainsKey("HoTen"))
            {
                ViewBag.TaiKhoan = HttpContext.Request.Cookies["HoTen"].ToString();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
            ViewBag.slnguoidung = _context.Accounts.Where(acc => acc.IsAdmin == false).Count();
            ViewBag.slgiohang = _context.Carts.GroupBy(cart => cart.AccountId).Count();
            DateTime date = DateTime.Today;
            var dayofweek = DayOfWeek.Sunday;
            var s = date.DayOfWeek;
            if (s != dayofweek)
            {
                var a = dayofweek - s;
                date = date.AddDays(a);
            }
            var hoadon = _context.Invoices.Include(i => i.Account).Where(i => i.IssuedDate.Date >= date.Date);
            var tatcahoadon = _context.Invoices.Include(i => i.Account);
            ViewBag.hoadon = hoadon;
            ViewBag.slhoadon = tatcahoadon.Count();
            ViewBag.slsanpham = _context.Products.Count();
            return View();
        }
        public IActionResult ThongKeChiTiet()
        {
            DateTime date = DateTime.Today;
            var dayofweek = DayOfWeek.Sunday;
            var s = date.DayOfWeek;
            if(s != dayofweek)
            {
                var a = dayofweek - s;
                date = date.AddDays(a);
            }
            var hoadon = _context.Invoices.Include(i => i.Account).Where(i=>i.IssuedDate.Date >= date.Date);
            ViewBag.hoadon = hoadon;

            var tiendachi = _context.Invoices.Include(i => i.Account).ToList();
            return View(tiendachi);
        }
        [HttpPost]
        public IActionResult ThongKeChiTiet(DateTime datebefore, DateTime dateafter)
        {
            ViewBag.doanhthu = _context.Invoices.Where(i => i.IssuedDate.Date >= datebefore.Date && i.IssuedDate.Date <= dateafter.Date).Sum(i => i.Total);
            DateTime date = DateTime.Today;
            var dayofweek = DayOfWeek.Sunday;
            var s = date.DayOfWeek;
            if (s != dayofweek)
            {
                var a = dayofweek - s;
                date = date.AddDays(a);
            }
            var hoadon = _context.Invoices.Include(i => i.Account).Where(i => i.IssuedDate.Date >= date.Date);
            ViewBag.hoadon = hoadon;

            var tiendachi = _context.Invoices.Include(i => i.Account).ToList();
            return View(tiendachi);
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(string Username, string Password)
        {
            Account taikhoan = _context.Accounts.Where(tkhoan => tkhoan.Username == Username && tkhoan.Password == Password && tkhoan.IsAdmin == true).FirstOrDefault();
            if (taikhoan != null)
            {
                CookieOptions cookieOptions = new CookieOptions()
                {
                    Expires = DateTime.Now.AddDays(7)
                };
                HttpContext.Response.Cookies.Append("TaiKhoanId", taikhoan.Id.ToString(), cookieOptions);
                HttpContext.Response.Cookies.Append("HoTen", taikhoan.FullName.ToString(), cookieOptions);
                return RedirectToAction("Index", "Home");
            }
            ViewBag.dangnhapthatbai = "Tài khoản hoặc mật khẩu không đúng";
            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Response.Cookies.Append("HoTen", "", new CookieOptions() { Expires = DateTime.Now.AddDays(-1) });
            HttpContext.Response.Cookies.Append("TaiKhoanId", "", new CookieOptions() { Expires = DateTime.Now.AddDays(-1) });
            return RedirectToAction("Login", "Home");
        }
    }
}
