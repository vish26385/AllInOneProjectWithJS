using AllInOneProject.Models;
using Microsoft.EntityFrameworkCore;

namespace AllInOneProject.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Item> Items { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<PartyMaster> PartyMasters { get; set; }
        public DbSet<SalesMaster> SalesMas { get; set; }
        public DbSet<SalesDetail> SalesDet { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<PurchaseMaster> PurchaseMaster { get; set; }
        public DbSet<PurchaseDetail> PurchaseDetails { get; set; }
    }
}
