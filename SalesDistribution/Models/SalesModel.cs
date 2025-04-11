using System.Collections.Concurrent;

namespace SalesDistribution.Models;

public class SalesModel : IModelBase
{
    public ConcurrentDictionary<string, Sale> Sales { get; set; } = [];

    public int Version { get; set; }


    public class Sale
    {
        public string Id { get; set; } = Common.GenerateId();

        public DateOnly SaleDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);

        public string Name { get; set; } = default!;

        public string? Description { get; set; }

        public string? Reference { get; set; }

        public List<string> Images { get; set; } = [];

        public List<SaleItem> Items { get; set; } = [];

        public int PurchasePrice { get; set; }

        public int SalePrice { get; set; }

        public int SalesCommission { get; set; }

        public decimal SalesCommissionPer { get; set; }

        public int IncomePrice { get; set; }

        public int SalesProfit { get; set; }

        public decimal SalesProfitRatio { get; set; }

        public SaleStatus Status { get; set;}

        public DateOnly? ShipDate { get; set; }

        public DateOnly? CloseDate { get; set; }
    }

    public class SaleItem
    {
        public string? Sku { get; set; }

        public string Name { get; set; }

        public decimal Amount { get; set; }

        public decimal PurchaseUnitPrice { get; set; }

        public decimal SaleUnitPrice { get; set; }

        public decimal PurchasePrice { get; set; }

        public decimal SalePrice { get; set; }
    }

    public enum SaleStatus
    {
        None,
        Sold,
        Selled,
        Shipped,
        Closed
    }
}
