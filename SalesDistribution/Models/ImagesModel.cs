namespace SalesDistribution.Models;

public class ImagesModel : IModelBase
{
    public List<ImageInfo> Images { get; set; } = [];

    public int Version { get; set; }

    public class ImageInfo
    {
        public string Id { get; set; } = default!;

        public HashSet<string> Refs { get; set; } = [];
    }
}
