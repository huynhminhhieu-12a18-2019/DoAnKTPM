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
        public ActionResult CategoryProducts(string SearchString = "")
        {
            if (HttpContext.Session.Keys.Contains("AccountUsername"))
            {
                ViewBag.AccountUserName = HttpContext.Session.GetString("AccountUsername");
            }
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
        public async Task<IActionResult> Cart()
        {
            string username = HttpContext.Session.GetString("AccountUsername");
            var carts = _context.Carts.Include(c => c.Account).Include(c => c.Product)
                                      .Where(c => c.Account.Username == username);
            ViewBag.Total = carts.Sum(c => c.Quantity * c.Product.Price);
            return View(await carts.ToListAsync());
        }
        public IActionResult Addcart(int id)
        {
            return Addcart(id, 1);
        }
        [HttpPost]
        public IActionResult Addcart(int productId, int quantity)
        {
            if (HttpContext.Session.GetString("AccountUsername") != null)
            {
                string username = HttpContext.Session.GetString("AccountUsername");
                int accountId = _context.Accounts.FirstOrDefault(a => a.Username == username).Id;
                Cart cart = _context.Carts.FirstOrDefault(c => c.AccountId == accountId && c.ProductId == productId);
                if (cart == null)
                {
                    cart = new Cart();
                    cart.AccountId = accountId;
                    cart.ProductId = productId;
                    cart.Quantity = quantity;
                    _context.Carts.Add(cart);
                }
                else
                {
                    cart.Quantity += quantity;
                }
                _context.SaveChanges();
                return RedirectToAction("Cart");
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }

        }
        /// xóa item trong cart
        [Route("/RemoveCart/{productid:int}", Name = "RemoveCart")]
        public IActionResult RemoveCart([FromRoute] int productid)
        {
            Cart cart = _context.Carts.FirstOrDefault();
            var cartitem = _context.Carts.Where(p => p.Product.Id == productid);
            foreach (Cart c in cartitem)
            {
                _context.Carts.Remove(c);
            }
            _context.SaveChanges();
            return RedirectToAction("Cart");
        }

        [HttpPost]
        [Route("/UpdateCart/{productid:int}", Name = "UpdateCart")]
        public IActionResult UpdateCart(int productid, int quantity)
        {
            Cart cart = _context.Carts.FirstOrDefault();
            var cartitem = _context.Carts.Where(p => p.Product.Id == productid);
            foreach (Cart c in cartitem)
            {
                c.Quantity = quantity;
            }
            _context.SaveChanges();
            return RedirectToAction("Cart");
        }
        public IActionResult RemoveAllCart()
        {
            Cart cart = _context.Carts.FirstOrDefault();
            var cartitem = _context.Carts;
            foreach (Cart c in cartitem)
            {
                _context.Carts.Remove(c);
            }
            _context.SaveChanges();
            return RedirectToAction("Cart");
        }
        public IActionResult Pay()
        {
            string username = HttpContext.Session.GetString("AccountUsername");
            ViewBag.Account = _context.Accounts.Where(a => a.Username == username).FirstOrDefault();
            ViewBag.CartsTotal = _context.Carts.Include(c => c.Product).Include(c => c.Account)
                                                .Where(c => c.Account.Username == username)
                                                .Sum(c => c.Quantity * c.Product.Price);
            return View("Pay");
        }
        [HttpPost]
        public IActionResult Pay([Bind("ShippingAddress,ShippingPhone")] Invoice invoice)
        {
            string username = HttpContext.Session.GetString("AccountUsername");
            if (!CheckStock(username))
            {
                ViewBag.ErrorMessage = "Có sản phẩm đã hết hàng. Vui lòng kiểm tra lại";
                ViewBag.Account = _context.Accounts.Where(a => a.Username == username).FirstOrDefault();
                ViewBag.CartsTotal = _context.Carts.Include(c => c.Product).Include(c => c.Account)
                                                    .Where(c => c.Account.Username == username)
                                                    .Sum(c => c.Quantity * c.Product.Price);
                return View("Pay");
            }
            //Thêm hoá đơn
            DateTime now = DateTime.Now;
            invoice.Code = now.ToString("yyMMddhhmmss");
            invoice.AccountId = _context.Accounts.FirstOrDefault(a => a.Username == username).Id;
            invoice.IssuedDate = now;
            invoice.Total = _context.Carts.Include(c => c.Product).Include(c => c.Account)
                                                    .Where(c => c.Account.Username == username)
                                                    .Sum(c => c.Quantity * c.Product.Price);
            _context.Add(invoice);
            _context.SaveChanges();
            //Thêm chi tiết hoá đơn
            List<Cart> carts = _context.Carts.Include(c => c.Product).Include(c => c.Account)
                                                    .Where(c => c.Account.Username == username).ToList();
            foreach (Cart c in carts)
            {
                InvoiceDetail invoiceDetail = new InvoiceDetail();
                invoiceDetail.InvoiceId = invoice.Id;
                invoiceDetail.ProductId = c.ProductId;
                invoiceDetail.Quantity = c.Quantity;
                invoiceDetail.UnitPrice = c.Product.Price;
                _context.Add(invoiceDetail);
            }
            _context.SaveChanges();
            //Trừ số lượng tồn kho và xoá giỏ hàng
            foreach (Cart c in carts)
            {
                c.Product.Stock -= c.Quantity;
                _context.Carts.Remove(c);
            }
            _context.SaveChanges();
            return RedirectToAction("Index", "Home");

        }
        private bool CheckStock(string username)
        {
            List<Cart> carts = _context.Carts.Include(c => c.Product).Include(c => c.Account)
                                                    .Where(c => c.Account.Username == username).ToList();
            foreach (Cart c in carts)
            {
                if (c.Product.Stock < c.Quantity)
                {
                    return false;
                }
            }
            return true;
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
                return RedirectToAction("Login", "Home");
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
