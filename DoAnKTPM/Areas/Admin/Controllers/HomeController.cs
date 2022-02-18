using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DoAnKTPM.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using DoAnKTPM.Models;
using Microsoft.AspNetCore.Authorization;

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
            ViewBag.slnguoidung = _context.Accounts.Where(acc => acc.IsAdmin == false).Count();
            ViewBag.slgiohang = _context.Carts.GroupBy(cart => cart.AccountId).Count();
            var hoadon = _context.Invoices.Include(i => i.Account);
            ViewBag.hoadon = hoadon;
            ViewBag.slhoadon = hoadon.Count();
            ViewBag.slsanpham = _context.Products.Count();
            return View();
        }
        public IActionResult ThongKeChiTiet()
        {
            var hoadon = _context.Invoices.Include(i => i.Account);
            ViewBag.hoadon = hoadon;

            var tiendachi = _context.Invoices.Include(i => i.Account).ToList();
            //var spbanchay = _context.InvoiceDetails.Include(invd => invd.Product).GroupBy(invd => invd.ProductId).Select(g => new
            //{
            //    TongSL = g.Sum(invd => invd.Quantity),
            //    MaSP = g.Select(invd => invd.Product.SKU),
            //    TenSP = g.Select(invd => invd.Product.Name)
            //});
            var spbanchay = _context.InvoiceDetails.Include(invd => invd.Product).ToList();
            var spbanchays = spbanchay.GroupBy(i => i.ProductId).SelectMany(cl => cl.Select(
                    csLine => new
                    {
                        MaSP = csLine.Product.SKU,
                        TenSP = csLine.Product.Name,
                        TongSL = cl.Sum(c => c.Quantity).ToString(),
                    })).ToList();
            ViewBag.spbanchay = spbanchays;
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
