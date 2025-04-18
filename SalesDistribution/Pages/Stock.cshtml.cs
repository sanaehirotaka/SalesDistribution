using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SalesDistribution.Models;
using SalesDistribution.Services;
using static SalesDistribution.Models.ItemsModel;
using static SalesDistribution.Models.StocksModel;

namespace SalesDistribution.Pages;

public class StockModel : PageModel
{
    private readonly Serializer serializer;

    public Stock Stock { get; set; }

    [BindProperty]
    public string Rute { get; set; } = "Update";

    [BindProperty]
    public string Sku { get; set; }

    [BindProperty]
    public StockHistory StockHistory { get; set; }

    [BindProperty]
    public string? Revoke { get; set; }

    public Item Item { get; set; }

    public StockModel(Serializer serializer)
    {
        this.serializer = serializer;
    }

    public async Task<IActionResult> OnGetAsync(string sku)
    {
        Sku = sku;
        var stocks = await serializer.ReadAsync<StocksModel>() ?? new();
        Stock = stocks.Stocks.GetOrAdd(Sku, key => new());
        var items = await serializer.ReadAsync<ItemsModel>() ?? new();
        Item = items.Items.GetOrAdd(Sku, key => new());

        StockHistory = new();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        return Rute switch
        {
            "Update" => await Update(),
            "Revoke" => await RevokeStock(),
            _ => Page()
        };
    }

    public static Stock Calc(Stock stock)
    {
        stock.PurchaseUnitPrice = 0;
        stock.TotalAmount = 0;

        foreach (var history in stock.Histories)
        {
            if ((stock.TotalAmount + history.Amount) > 0 && history.Amount > 0)
            {
                stock.PurchaseUnitPrice = ((stock.PurchaseUnitPrice * stock.TotalAmount) + (history.UnitPrice!.Value * history.Amount)) / (stock.TotalAmount + history.Amount);
            }
            stock.TotalAmount += history.Amount;
        }
        return stock;
    }

    private async Task<IActionResult> Update()
    {
        var stocks = await serializer.ReadAsync<StocksModel>() ?? new();
        Stock = stocks.Stocks.GetOrAdd(Sku, key => new());
        var items = await serializer.ReadAsync<ItemsModel>() ?? new();
        Item = items.Items.GetOrAdd(Sku, key => new());

        if (!string.IsNullOrEmpty(StockHistory.Reference) && !Uri.TryCreate(StockHistory.Reference, UriKind.Absolute, out var _))
        {
            ModelState.AddModelError($"StockHistory.Reference", "URL形式で入力してください");
        }
        if (StockHistory.Amount == 0)
        {
            ModelState.AddModelError($"StockHistory.Amount", "数量は0以外で入力してください");
        }
        if (!ModelState.IsValid) // バリデーションチェック
        {
            return Page();
        }

        if (StockHistory.UnitPrice != null)
        {
            StockHistory.Price = StockHistory.UnitPrice * StockHistory.Amount;
        }
        else if (StockHistory.Price != null)
        {
            StockHistory.UnitPrice = StockHistory.Price / StockHistory.Amount;
        }
        Stock.Histories.Add(StockHistory);

        Calc(Stock);
        await serializer.WriteAsync(stocks);

        ModelState.Clear();
        StockHistory = new();

        return Page();
    }

    private async Task<IActionResult> RevokeStock()
    {
        var stocks = await serializer.ReadAsync<StocksModel>() ?? new();
        Stock = stocks.Stocks.GetOrAdd(Sku, key => new());
        var items = await serializer.ReadAsync<ItemsModel>() ?? new();
        Item = items.Items.GetOrAdd(Sku, key => new());

        Stock.Histories.RemoveAll(x => x.Id == Revoke);

        Calc(Stock);
        await serializer.WriteAsync(stocks);

        return Page();
    }
}
