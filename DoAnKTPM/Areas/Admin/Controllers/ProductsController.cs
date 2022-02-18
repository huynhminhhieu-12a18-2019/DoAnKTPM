using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DoAnKTPM.Data;
using DoAnKTPM.Models;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace DoAnKTPM.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly DoAnKTPMContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductsController(DoAnKTPMContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Admin/Products
        public async Task<IActionResult> Index()
        {
            var doAnKTPMContext = _context.Products.Include(p => p.ProductType);
            return View(await doAnKTPMContext.ToListAsync());
        }

        // GET: Admin/Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.ProductType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Admin/Products/Create
        public IActionResult Create()
        {
            ViewData["ProductTypeId"] = new SelectList(_context.ProductTypes, "Id", "Name");
            return View();
        }

        // POST: Admin/Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SKU,Name,Description,Price,Stock,ProductTypeId,Image,ImageFile,Status")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                if (product.ImageFile != null)
                {
                    var filename = product.Id.ToString() + Path.GetExtension(product.ImageFile.FileName);
                    var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "img", "product");
                    var filePath = Path.Combine(uploadPath, filename);
                    using (FileStream fs = System.IO.File.Create(filePath))
                    {
                        product.ImageFile.CopyTo(fs);
                        fs.Flush();
                    }
                    product.Image = filename;
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductTypeId"] = new SelectList(_context.ProductTypes, "Id", "Name", product.ProductTypeId);
            return View(product);
        }

        // GET: Admin/Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["ProductTypeId"] = new SelectList(_context.ProductTypes, "Id", "Name", product.ProductTypeId);
            return View(product);
        }

        // POST: Admin/Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SKU,Name,Description,Price,Stock,ProductTypeId,Image,ImageFile,Status")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _context.SaveChangesAsync();
                    if (product.ImageFile != null)
                    {
                        var filename = product.Id.ToString() + Path.GetExtension(product.ImageFile.FileName);
                        var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "img", "product");
                        var filePath = Path.Combine(uploadPath, filename);
                        using (FileStream fs = System.IO.File.Create(filePath))
                        {
                            product.ImageFile.CopyTo(fs);
                            fs.Flush();
                        }
                        product.Image = filename;
                        _context.Update(product);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductTypeId"] = new SelectList(_context.ProductTypes, "Id", "Name", product.ProductTypeId);
            return View(product);
        }

        // GET: Admin/Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.ProductType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Admin/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
