using Microsoft.EntityFrameworkCore;
using Stock.Api.Infrastructure.Persistence.Configurations;

namespace Stock.Api.Infrastructure.Persistence;

public class StockDbContext : DbContext
{
    public StockDbContext(DbContextOptions<StockDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    public DbSet<Domain.Stocks.Stock> Stocks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        new StockConfiguration().Configure(modelBuilder.Entity<Domain.Stocks.Stock>());

        base.OnModelCreating(modelBuilder);
    }
}
