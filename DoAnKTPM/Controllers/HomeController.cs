using DoAnKTPM.Data;
using DoAnKTPM.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DoAnKTPM.Controllers
{
    public class HomeController : Controller
    {
        private readonly DoAnKTPMContext _context;

        public HomeController(DoAnKTPMContext context)
        {
            _context = context;
        }

        // GET: Products
        public ActionResult Index(string SearchString = "")
        {
            if (HttpContext.Session.Keys.Contains("AccountUsername"))
            {
                ViewBag.AccountUserName = HttpContext.Session.GetString("AccountUsername");
            }
            List<Product> products;
            if (SearchString != "" && SearchString != null)
            {
                products = _context.Products.Include(p => p.ProductType)
                .Where(p => p.Name.Contains(SearchString) || p.SKU.Contains(SearchString) || p.Description.Contains(SearchString))
                .ToList();
            }
            else
                products = _context.Products.ToList();
            return View(products);
        }
        public ActionResult AllProducts(string SearchString = "")
        {
            List<Product> products;
            if (SearchString != "" && SearchString != null)
            {
                products = _context.Products.Include(p => p.ProductType)
                .Where(p => p.Name.Contains(SearchString) || p.SKU.Contains(SearchString) || p.Description.Contains(SearchString))
                .ToList();
            }
            else
                products = _context.Products.ToList();
            return View(products);
        }
        public ActionResult CategoryProducts(string SearchString = "")
        {
            List<Product> products;
            if (SearchString != "" && SearchString != null)
            {
                products = _context.Products.Include(p => p.ProductType)
                .Where(p => p.ProductTypeId.ToString().Contains(SearchString))
                .ToList();
            }
            else
                products = _context.Products.ToList();
            return View(products);
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(string Username, string Password)
        {
            Account account = _context.Accounts.Where(a => a.Username == Username && a.Password == Password).FirstOrDefault();
            if (account != null)
            {
                //Tao Session
                HttpContext.Session.SetInt32("AccountID", account.Id);
                HttpContext.Session.SetString("AccountUsername", account.Username);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.ErrorMessage = "Đăng nhập thất bại";
                return View();
            }
        }
        //Xu ly Logout
        public IActionResult Logout()
        {
            //Xoa Session
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
        public IActionResult Register()
        {
            return View();
        }

        // POST: Accounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("Id,Username,Password")] Account account)
        {

            if (ModelState.IsValid)
            {
                _context.Add(account);
                await _context.SaveChangesAsync();
                return RedirectToAction("Login","Home");
            }
            return View(account);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
