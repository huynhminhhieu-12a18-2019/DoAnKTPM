using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DoAnKTPM.Models;

namespace DoAnKTPM.Data
{
    public class DoAnKTPMContext : DbContext
    {
        public DoAnKTPMContext (DbContextOptions<DoAnKTPMContext> options)
            : base(options)
        {
        }

        public DbSet<DoAnKTPM.Models.Account> Accounts { get; set; }

        public DbSet<DoAnKTPM.Models.Cart> Carts { get; set; }

        public DbSet<DoAnKTPM.Models.Invoice> Invoices { get; set; }

        public DbSet<DoAnKTPM.Models.InvoiceDetail> InvoiceDetails { get; set; }

        public DbSet<DoAnKTPM.Models.Product> Products { get; set; }

        public DbSet<DoAnKTPM.Models.ProductType> ProductTypes { get; set; }
    }
}
