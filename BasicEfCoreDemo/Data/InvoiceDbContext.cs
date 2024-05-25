using BasicEfCoreDemo.Models;
using Microsoft.EntityFrameworkCore;

namespace BasicEfCoreDemo.Data
{
    public class InvoiceDbContext(DbContextOptions<InvoiceDbContext> options) : DbContext(options)
    {
        //public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Invoice> Invoices => Set<Invoice>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Invoice>().HasData(
            new Invoice
            {
                Id = Guid.NewGuid(),
                InvoiceNumber = "INV-001",
                ContactName = "Iron Man",
                Description = "Invoice for the first month",
                Amount = 100,
                InvoiceDate = new DateTimeOffset(2023, 1, 1, 0, 0, 0,
           TimeSpan.Zero),
                DueDate = new DateTimeOffset(2023, 1, 15, 0, 0, 0,
           TimeSpan.Zero),
                Status = InvoiceStatus.AwaitPayment
            });
        }
    }
}
