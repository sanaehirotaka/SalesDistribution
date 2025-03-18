using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SalesDistribution.Models;
using SalesDistribution.Services;

namespace SalesDistribution.Pages;

public class SaleListModel : PageModel
{
    private readonly Serializer serializer;

    public SalesModel Sales { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Status { get; set; }

    public SaleListModel(Serializer serializer)
    {
        this.serializer = serializer;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        Sales = await serializer.ReadAsync<SalesModel>() ?? new();
        return Page();
    }
}
