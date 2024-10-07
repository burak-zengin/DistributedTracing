using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Stock.Api.Infrastructure.Persistence.Configurations;

public class StockConfiguration : IEntityTypeConfiguration<Domain.Stocks.Stock>
{
    public void Configure(EntityTypeBuilder<Domain.Stocks.Stock> builder)
    {
        builder
            .HasKey(_ => _.Barcode);

        builder
            .HasData(new List<Domain.Stocks.Stock>
            {
                new Domain.Stocks.Stock
            {
                Barcode = "123456",
                Quantity = 1000
            }, new Domain.Stocks.Stock
            {
                Barcode = "234567",
                Quantity = 1000
            }, new Domain.Stocks.Stock
            {
                Barcode = "345678",
                Quantity = 1000
            }
            });
    }
}
