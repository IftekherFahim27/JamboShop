using JamboShop.Models;
using Microsoft.EntityFrameworkCore;

namespace JamboShop.Data
{
    public class ERPDbContext:DbContext
    {
        public ERPDbContext(DbContextOptions<ERPDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
    }
}
