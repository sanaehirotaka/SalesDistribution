namespace SalesDistribution.Models;

public class OptionsModel : IModelBase
{
    public string DataLocation { get; set; } = default!;
    public int Version { get; set; }

    public OptionsModel(IConfiguration configuration)
    {
        configuration.Bind(nameof(OptionsModel), this);
    }
}
