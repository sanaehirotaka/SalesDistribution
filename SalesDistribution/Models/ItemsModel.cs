using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;

namespace SalesDistribution.Models;

public class ItemsModel : IModelBase
{
    public ConcurrentDictionary<string, Item> Items { get; set; } = [];

    public int Version { get; set; }

    public class Item
    {
        [Required]
        public string Sku { get; set; } = default!;

        [Required]
        public string Name { get; set; } = default!;

        [Required]
        public string UnitName { get; set; } = default!;

        public string? Description { get; set; }

        [Required]
        public bool PurchaseTarget { get; set; }

        [Required]
        public bool SalesTarget { get; set; }

        [Required]
        public bool BundleTarget { get; set; }

        public List<BundleItem> Bundles { get; set; } = [];
    }

    public class BundleItem
    {
        public int Order { get; set; } = default!;

        public string Sku { get; set; } = default!;

        public string Name { get; set; } = default!;

        public decimal Amount { get; set; }
    }
}