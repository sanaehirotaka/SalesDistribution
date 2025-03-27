using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SalesDistribution.Models;
using SalesDistribution.Services;
using System;

namespace SalesDistribution.Pages;

public class SaleModel : PageModel
{
    private readonly Serializer serializer;

    private readonly StockService stockService;

    [BindProperty]
    public string Rute { get; set; } = "Update";

    public bool IsNew { get; set; } = true;

    [BindProperty]
    public SalesModel.Sale Sale { get; set; }

    [BindProperty]
    public int? RemoveItemRow { get; set; }

    public SaleModel(Serializer serializer, StockService stockService)
    {
        this.serializer = serializer;
        this.stockService = stockService;
    }

    public async Task<IActionResult> OnGetAsync(string? id)
    {
        if (id == null)
        {
            Sale = new();
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
        foreach (var view in Sale.Items.Select((item, index) => (Item : item, Index : index)))
        {
            if (Sale.Items.Count(item => item.Sku == view.Item.Sku) > 1)
            {
                ModelState.AddModelError($"Sale.Items[{view.Index}].Sku", "品目が重複しています");
            }
        }
        if (!ModelState.IsValid) // バリデーションチェック
        {
            return Page();
        }

        var sales = await serializer.ReadAsync<SalesModel>() ?? new();

        if (sales.Sales.TryGetValue(Sale.Id, out var oldSale))
        {
            foreach (var saleItem in oldSale.Items.Where(item => !Sale.Items.Any(v => v.Sku == item.Sku)))
            {
                await stockService.PopSaleStock(saleItem.Sku!, Sale.Id);
            }
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
        return RedirectToPage("SaleList");
    }

    private async Task<IActionResult> Remove()
    {
        var sales = await serializer.ReadAsync<SalesModel>() ?? new();
        if (sales.Sales.TryGetValue(Sale.Id, out var sale))
        {
            foreach (var saleItem in sale.Items)
            {
                await stockService.PopSaleStock(saleItem.Sku!, Sale.Id);
            }
            sales.Sales.Remove(Sale.Id, out var _);
        }
        await serializer.WriteAsync(sales);
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
