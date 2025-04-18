using SalesDistribution.Models;
using SalesDistribution.Pages;

namespace SalesDistribution.Services;

public class StockService
{
    private readonly Serializer serializer;

    private StocksModel stocks;

    private ItemsModel items;

    public StockService(Serializer serializer)
    {
        this.serializer = serializer;
    }

    public async Task<int?> GetStockAsync(string? sku)
    {
        if (string.IsNullOrEmpty(sku))
        {
            return null;
        }

        stocks ??= await serializer.ReadAsync<StocksModel>() ?? new();
        var stock = stocks.Stocks.GetOrAdd(sku, key => new());

        items ??= await serializer.ReadAsync<ItemsModel>() ?? new();
        var item = items.Items.GetOrAdd(sku, key => new());

        if (item.PurchaseTarget)
        {
            return (int)stock.TotalAmount;
        }
        if (item.Bundles.Count > 0)
        {
            return (await Task.WhenAll(item.Bundles.Select(async bundle => (int)((await GetStockAsync(bundle.Sku) ?? 0) / bundle.Amount)))).Min();
        }
        return null;
    }

    public async Task<int?> GetUnitPriceAsync(string? sku)
    {
        if (string.IsNullOrEmpty(sku))
        {
            return null;
        }

        stocks ??= await serializer.ReadAsync<StocksModel>() ?? new();
        var stock = stocks.Stocks.GetOrAdd(sku, key => new());

        items ??= await serializer.ReadAsync<ItemsModel>() ?? new();
        var item = items.Items.GetOrAdd(sku, key => new());

        if (item.PurchaseTarget)
        {
            return (int)stock.PurchaseUnitPrice;
        }
        if (item.Bundles.Count > 0)
        {
            return (await Task.WhenAll(item.Bundles.Select(async item => (int)((await GetUnitPriceAsync(item.Sku) ?? 0) * item.Amount)))).Sum();
        }
        return null;
    }

    public async Task PushSaleStock(string sku, StocksModel.StockHistory pushHistory)
    {
        stocks ??= await serializer.ReadAsync<StocksModel>() ?? new();
        var stock = stocks.Stocks.GetOrAdd(sku, key => new());

        items ??= await serializer.ReadAsync<ItemsModel>() ?? new();
        var item = items.Items.GetOrAdd(sku, key => new());

        if (item.PurchaseTarget)
        {
            var history = stock.Histories.Find(h => h.SaleId == pushHistory.SaleId);
            if (history == null)
            {
                history = new()
                {
                    SaleId = pushHistory.SaleId,
                };
                stock.Histories.Add(history);
            }
            history.Amount = pushHistory.Amount;
            StockModel.Calc(stock);
        }
        else
        {
            foreach (var bundle in item.Bundles)
            {
                var bundleItem = items.Items.GetOrAdd(bundle.Sku, key => new());
                var bundleItemStock = stocks.Stocks.GetOrAdd(bundle.Sku, key => new());

                var history = bundleItemStock.Histories.Find(h => h.SaleId == pushHistory.SaleId);
                if (history == null)
                {
                    history = new()
                    {
                        SaleId = pushHistory.SaleId,
                    };
                    bundleItemStock.Histories.Add(history);
                }
                history.Amount = bundle.Amount * pushHistory.Amount;
                StockModel.Calc(bundleItemStock);
            }
        }
        await serializer.WriteAsync(stocks);
    }

    public async Task PopSaleStock(string sku, string saleId)
    {
        stocks ??= await serializer.ReadAsync<StocksModel>() ?? new();
        var stock = stocks.Stocks.GetOrAdd(sku, key => new());

        items ??= await serializer.ReadAsync<ItemsModel>() ?? new();
        var item = items.Items.GetOrAdd(sku, key => new());

        if (item.PurchaseTarget)
        {
            stock.Histories.RemoveAll(h => h.SaleId == saleId);
            StockModel.Calc(stock);
        }
        else
        {
            foreach (var bundle in item.Bundles)
            {
                var bundleItemStock = stocks.Stocks.GetOrAdd(bundle.Sku, key => new());
                bundleItemStock.Histories.RemoveAll(h => h.SaleId == saleId);
                StockModel.Calc(bundleItemStock);
            }
        }
        await serializer.WriteAsync(stocks);
    }
}
