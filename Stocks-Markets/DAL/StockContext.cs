using Microsoft.EntityFrameworkCore;
using Stocks_Markets.Models;

namespace Stocks_Markets.DAL
{
    public class StockContext : DbContext
    {
        public StockContext(DbContextOptions<StockContext> options) : base (options)
        {
            Database.EnsureCreated();
        }
        //Create Tables inside DB
        public DbSet<Stock> stocks { get; set; }

        public DbSet<BrukerStocks> brukerstock { get; set; }

        public DbSet<Brukere> brukere { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
        }
    }
}
