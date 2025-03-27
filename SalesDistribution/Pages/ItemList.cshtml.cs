using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SalesDistribution.Models;
using SalesDistribution.Services;

namespace SalesDistribution.Pages;

public class ItemListModel : PageModel
{
    private readonly Serializer serializer;

    public ItemsModel Items { get; set; }

    public ItemListModel(Serializer serializer)
    {
        this.serializer = serializer;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        Items = await serializer.ReadAsync<ItemsModel>() ?? new();
        return Page();
    }
}
