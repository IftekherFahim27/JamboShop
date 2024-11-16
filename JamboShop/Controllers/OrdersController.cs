using JamboShop.Data;
using JamboShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JamboShop.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ERPDbContext _context;

        public OrdersController(ERPDbContext context)
        {
            _context = context;
        }

        // API 1: Create an order
        [HttpPost]
        public async Task<IActionResult> CreateOrder(Order order)
        {
            var product = await _context.Products.FindAsync(order.ProductId);
            if (product == null || product.Stock < order.Quantity)
            {
                return BadRequest("Insufficient stock.");
            }

            product.Stock -= order.Quantity;
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return Ok(order);
        }

        // API 2: Update order quantity
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, decimal newQuantity)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();

            var product = await _context.Products.FindAsync(order.ProductId);
            var difference = newQuantity - order.Quantity;

            if (product.Stock < difference)
            {
                return BadRequest("Insufficient stock.");
            }

            product.Stock -= difference;
            order.Quantity = newQuantity;

            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            return Ok(order);
        }

        // API 3: Delete an order
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();

            var product = await _context.Products.FindAsync(order.ProductId);
            product.Stock += order.Quantity;

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return Ok();
        }

        // API 4: Get all orders with product details
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _context.Orders
                .Include(o => o.Product)
                .ToListAsync();
            return View(orders);
        }

        // API 5: Get product summary
        public async Task<IActionResult> ProductSummary()
        {
            var summary = await _context.Products
                .Select(p => new
                {
                    ProductName = p.ProductName,
                    TotalQuantity = _context.Orders.Where(o => o.ProductId == p.ProductId).Sum(o => o.Quantity),
                    TotalRevenue = _context.Orders.Where(o => o.ProductId == p.ProductId).Sum(o => o.Quantity * p.UnitPrice)
                })
                .ToListAsync();

            return View(summary);
        }

        // API 9: Bulk order transaction
        [HttpPost]
        public async Task<IActionResult> BulkOrder(List<Order> orders)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var order in orders)
                {
                    var product = await _context.Products.FindAsync(order.ProductId);
                    if (product == null || product.Stock < order.Quantity)
                    {
                        throw new Exception("Insufficient stock for one or more orders.");
                    }

                    product.Stock -= order.Quantity;
                    _context.Orders.Add(order);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return Ok("Orders created successfully.");
            }
            catch
            {
                await transaction.RollbackAsync();
                return BadRequest("Failed to create orders. Transaction rolled back.");
            }
        }
    }
}
