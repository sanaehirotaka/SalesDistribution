using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SalesDistribution.Models;
using SalesDistribution.Services;
using static SalesDistribution.Models.ItemsModel;

namespace SalesDistribution.Pages;

public class ItemModel : PageModel
{
    private readonly Serializer serializer;

    private readonly StockService stockService;

    [BindProperty]
    public string Rute { get; set; } = "Update";

    [BindProperty]
    public Item Item { get; set; } = default!;

    [BindProperty]
    public bool IsNew { get; set; } = true;

    [BindProperty]
    public int? RemoveBundleRow { get; set; }

    public ItemModel(Serializer serializer, StockService stockService)
    {
        this.serializer = serializer;
        this.stockService = stockService;
    }

    public async Task<IActionResult> OnGetAsync(string? sku)
    {
        if (sku != null)
        {
            var items = await serializer.ReadAsync<ItemsModel>() ?? new();
            if (items.Items.TryGetValue(sku, out var item))
            {
                Item = item;
                Item.Bundles.ForEach(b => b.Name = items.Items.GetValueOrDefault(b.Sku)?.Name ?? "???");
                IsNew = false;
            }
        }
        Item ??= new()
        {
            Sku = Common.GenerateId(),
            Name = string.Empty
        };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        return Rute switch
        {
            "Update" => await Update(),
            "Remove" => await Remove(),
            "RemoveBundleRow" => await RemoveRow(),
            "AddBundleRow" => await AddRow(),
            _ => Page()
        };
    }

    private async Task<IActionResult> Update()
    {
        if (!Item.PurchaseTarget && !Item.SalesTarget)
        {
            ModelState.AddModelError("", "仕入の対象と販売の対象のどちらか指定してください");
        }
        if (Item.PurchaseTarget && Item.BundleTarget)
        {
            ModelState.AddModelError("", "仕入の対象とバンドルの対象は両立できません");
        }
        if (Item.BundleTarget && Item.Bundles.Count == 0)
        {
            ModelState.AddModelError("", "バンドルが入力されていません");
        }
        if (!ModelState.IsValid)
        {
            return Page();
        }
        Item.Bundles = Item.Bundles.Where(b => !string.IsNullOrEmpty(b.Sku) && b.Amount > 0).ToList();
        var items = await serializer.ReadAsync<ItemsModel>() ?? new();
        items.Items[Item.Sku] = Item;
        await serializer.WriteAsync(items);

        Item = new()
        {
            Sku = Guid.NewGuid().ToString(),
            Name = string.Empty
        };
        return RedirectToPage("ItemList");
    }

    private async Task<IActionResult> Remove()
    {
        var stock = await stockService.GetStockAsync(Item.Sku);
        if (stock != null && stock != 0)
        {
            ModelState.AddModelError("", "在庫が0ではない品目は削除できません");
        }
        var sales = await serializer.ReadAsync<SalesModel>() ?? new();
        if (sales.Sales.Values.Where(sale => sale.Status != SalesModel.SaleStatus.Closed)
            .Where(sale => sale.Items.Any(item => item.Sku == Item.Sku))
            .Any())
        {
            ModelState.AddModelError("", "完了していない販売で参照されています");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }
        var items = await serializer.ReadAsync<ItemsModel>() ?? new();
        items.Items.Remove(Item.Sku, out var _);
        await serializer.WriteAsync(items);
        return RedirectToPage("ItemList");
    }

    private async Task<IActionResult> RemoveRow()
    {
        ModelState.Clear();
        Item.Bundles.RemoveAt(RemoveBundleRow!.Value);
        return Page();
    }

    private async Task<IActionResult> AddRow()
    {
        ModelState.Clear();
        Item.Bundles.Add(new() { Amount = 1 });
        return Page();
    }
}
