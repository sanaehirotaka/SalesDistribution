using Microsoft.AspNetCore.Mvc;
using SalesDistribution.Models;
using SalesDistribution.Services;

namespace SalesDistribution.Apis;

[Route("api/[controller]/[action]")]
[ApiController]
public class ItemController : ControllerBase
{
    private readonly Serializer serializer;
    private readonly StockService stockService;

    public ItemController(Serializer serializer, StockService stockService)
    {
        this.serializer = serializer;
        this.stockService = stockService;
    }

    [HttpPost]
    [Produces("application/json")]
    public async Task<List<Item>> Search([FromForm] string? Name, [FromForm] bool? IsPurchaseTarget, [FromForm] bool? IsSalesTarget, [FromForm] bool? IsBundleTarget)
    {
        var items = await serializer.ReadAsync<ItemsModel>() ?? new();

        return (await Task.WhenAll(items.Items.Values.Where(item => {
            if (!string.IsNullOrEmpty(Name))
                if (!item.Name.Contains(Name) && !item.Sku.Contains(Name))
                    return false;
            if (IsPurchaseTarget != null && IsPurchaseTarget != item.PurchaseTarget)
                return false;
            if (IsSalesTarget != null && IsSalesTarget != item.SalesTarget)
                return false;
            if (IsBundleTarget != null && IsBundleTarget !=!item.BundleTarget)
                return false;

            return true;
        }).Select(async item => new Item()
        {
            Sku = item.Sku,
            Name = item.Name,
            Stock = await stockService.GetStockAsync(item.Sku),
            UnitPrice = await stockService.GetUnitPriceAsync(item.Sku),
        }))).ToList();
    }


    public class Item
    {
        public string Sku { get; set; } = default!;

        public string Name { get; set; } = default!;

        public int? Stock { get; set; }

        public int? UnitPrice { get; set; }

    }
}
