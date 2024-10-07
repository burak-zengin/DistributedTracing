using Microsoft.EntityFrameworkCore;
using Ordering.Api.Domain.Orders;

namespace Ordering.Api.Infrastructure.Persistence;

public class OrderingDbContext(DbContextOptions<OrderingDbContext> options) : DbContext(options)
{
    public DbSet<Order> Orders { get; set; }

    public DbSet<Line> Lines { get; set; }
}
