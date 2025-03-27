using Microsoft.AspNetCore.Mvc;

namespace SalesDistribution.Apis;

[Route("api/[controller]/[action]")]
[ApiController]
public class SaleController : ControllerBase
{
    [HttpPost]
    [Produces("application/json")]
    public async Task<Sale> Calc([FromForm] Sale sale)
    {
        foreach (var item in sale.Items)
        {
            item.PurchasePrice = Math.Round(item.Amount * item.PurchaseUnitPrice, 3);
            item.SalePrice = Math.Round(item.Amount * item.SaleUnitPrice);
        }
        sale.PurchasePrice = (int)Math.Round(sale.Items.Select(item => item.Amount * item.PurchaseUnitPrice).Sum());
        sale.SalePrice = (int)Math.Round(sale.Items.Select(item => item.Amount * item.SaleUnitPrice).Sum());
        sale.IncomePrice = sale.SalePrice - (int)(sale.SalesCommission + (sale.SalePrice * (sale.SalesCommissionPer / 100)));
        sale.SalesProfit = sale.IncomePrice - sale.PurchasePrice;

        return sale;
    }

    public class Sale
    {
        public List<SaleItem> Items { get; set; } = [];

        public int PurchasePrice { get; set; }

        public int SalePrice { get; set; }

        public int SalesCommission { get; set; }

        public decimal SalesCommissionPer { get; set; }

        public int IncomePrice { get; set; }

        public int SalesProfit { get; set; }
    }

    public class SaleItem
    {
        public decimal Amount { get; set; }

        public decimal PurchaseUnitPrice { get; set; }

        public decimal SaleUnitPrice { get; set; }

        public decimal PurchasePrice { get; set; }

        public decimal SalePrice { get; set; }
    }
}
