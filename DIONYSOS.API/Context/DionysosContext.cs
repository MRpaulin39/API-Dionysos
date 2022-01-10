using DIONYSOS.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DIONYSOS.API.Context
{
    public class DionysosContext : DbContext
    {
        public DionysosContext(DbContextOptions<DionysosContext> options) : base(options)
        {

        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Alcohol> Alcohol { get; set; }
        public DbSet<OrderHeader> OrderHeader { get; set; }
        public DbSet<OrderLine> OrderLine { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<Supplier> Supplier { get; set; }
        public DbSet<OrderSupplier> OrderSupplier { get; set; }

    }
}
