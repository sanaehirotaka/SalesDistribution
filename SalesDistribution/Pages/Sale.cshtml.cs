using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SalesDistribution.Models;
using SalesDistribution.Services;

namespace SalesDistribution.Pages;

public class SaleModel : PageModel
{
    private readonly Serializer serializer;

    private readonly StockService stockService;

    private readonly ImageService imageService;

    [BindProperty(SupportsGet = true)]
    public string? CopySource { get; set; }

    [BindProperty]
    public string Rute { get; set; } = "Update";

    public bool IsNew { get; set; } = true;

    [BindProperty]
    public SalesModel.Sale Sale { get; set; }

    [BindProperty]
    public int? RemoveItemRow { get; set; }

    public SaleModel(Serializer serializer, StockService stockService, ImageService imageService)
    {
        this.serializer = serializer;
        this.stockService = stockService;
        this.imageService = imageService;
    }

    public async Task<IActionResult> OnGetAsync(string? id)
    {
        if (id == null)
        {
            Sale = new();
            if (CopySource != null)
            {
                var sales = await serializer.ReadAsync<SalesModel>() ?? new();
                Sale = sales.Sales.GetValueOrDefault(CopySource) ?? new();
                Sale.Id = Common.GenerateId();
                Sale.Status = SalesModel.SaleStatus.None;
                Sale.ShipDate = null;
                Sale.CloseDate = null;
            }
        }
        else
        {
            var sales = await serializer.ReadAsync<SalesModel>() ?? new();
            Sale = sales.Sales.GetOrAdd(id, id => new() { Id = id });
            IsNew = false;
        }
        return Page();
    }
    public async Task<IActionResult> OnPostAsync()
    {
        return Rute switch
        {
            "Update" => await Update(),
            "Remove" => await Remove(),
            "RemoveItemRow" => await RemoveRow(),
            "AddItemRow" => await AddRow(),
            _ => Page()
        };
    }

    private async Task<IActionResult> Update()
    {
        foreach (var view in Sale.Items.Select((item, index) => (Item: item, Index: index)))
        {
            if (Sale.Items.Count(item => item.Sku == view.Item.Sku) > 1)
            {
                ModelState.AddModelError($"Sale.Items[{view.Index}].Sku", "品目が重複しています");
            }
        }
        if (Sale.Items.Count == 0)
        {
            ModelState.AddModelError($"", "品目がありません");
        }
        if (Sale.SalePrice < 300)
        {
            ModelState.AddModelError($"Sale.SalePrice", "販売金額は300円以上である必要があります");
        }
        if (!ModelState.IsValid) // バリデーションチェック
        {
            return Page();
        }

        var sales = await serializer.ReadAsync<SalesModel>() ?? new();
        var oldImages = new List<string>();
        if (sales.Sales.TryGetValue(Sale.Id, out var oldSale))
        {
            foreach (var saleItem in oldSale.Items.Where(item => !Sale.Items.Any(v => v.Sku == item.Sku)))
            {
                if (saleItem.Sku != null)
                {
                    await stockService.PopSaleStock(saleItem.Sku, Sale.Id);
                }
            }
            oldImages = oldSale.Images;
        }
        sales.Sales[Sale.Id] = Sale;

        await serializer.WriteAsync(sales);
        foreach (var item in Sale.Items)
        {
            if (!string.IsNullOrEmpty(item.Sku))
            {
                await stockService.PushSaleStock(item.Sku, new()
                {
                    SaleId = Sale.Id,
                    Amount = -item.Amount,
                });
            }
        }
        foreach (var imageId in Sale.Images)
        {
            await imageService.PutAsync(imageId, Sale.Id);
        }
        foreach (var oldId in oldImages.Where(oldId => !Sale.Images.Contains(oldId)))
        {
            await imageService.DeleteAsync(oldId, Sale.Id);
        }
        return RedirectToPage("SaleList");
    }

    private async Task<IActionResult> Remove()
    {
        var sales = await serializer.ReadAsync<SalesModel>() ?? new();
        if (sales.Sales.TryGetValue(Sale.Id, out var sale))
        {
            foreach (var saleItem in sale.Items)
            {
                if (saleItem.Sku != null)
                {
                    await stockService.PopSaleStock(saleItem.Sku, Sale.Id);
                }
            }
            sales.Sales.Remove(Sale.Id, out var _);
        }
        await serializer.WriteAsync(sales);

        foreach (var imageId in Sale.Images)
        {
            await imageService.DeleteAsync(imageId, Sale.Id);
        }
        return RedirectToPage("SaleList");
    }

    private async Task<IActionResult> RemoveRow()
    {
        ModelState.Clear();
        Sale.Items.RemoveAt(RemoveItemRow!.Value);
        return Page();
    }

    private async Task<IActionResult> AddRow()
    {
        ModelState.Clear();
        Sale.Items.Add(new() { Amount = 1 });
        return Page();
    }
}
