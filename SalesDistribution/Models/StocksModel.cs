using System.Collections.Concurrent;

namespace SalesDistribution.Models;

public class StocksModel : IModelBase
{
    public ConcurrentDictionary<string, Stock> Stocks { get; set; } = [];

    public int Version { get; set; }

    public class Stock
    {
        public decimal TotalAmount { get; set; }

        public decimal PurchaseUnitPrice { get; set; }

        public List<StockHistory> Histories { get; set; } = [];
    }

    public class StockHistory
    {
        public string Id { get; set; } = Common.GenerateId();

        public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Now);

        public string? SaleId { get; set; }

        public string? Description { get; set; }

        public string? Reference { get; set; }

        public decimal Amount { get; set; }

        public decimal? Price { get; set; }

        public decimal? UnitPrice { get; set; }
    }
}
