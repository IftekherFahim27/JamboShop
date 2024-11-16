using JamboShop.Data;
using JamboShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JamboShop.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ERPDbContext _context;

        public ProductsController(ERPDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Products.ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Products.Update(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // API 6: Get products below stock threshold
        public async Task<IActionResult> LowStock(int threshold = 100)
        {
            var products = await _context.Products
                .Where(p => p.Stock < threshold)
                .ToListAsync();
            return View(products);
        }

        // API 8: Get products not ordered
        public async Task<IActionResult> NotOrdered()
        {
            var products = await _context.Products
                .Where(p => !_context.Orders.Any(o => o.ProductId == p.ProductId))
                .ToListAsync();
            return View(products);
        }

    }
}
