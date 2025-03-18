using SalesDistribution.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace SalesDistribution.Services;

public class ImageService
{
    private readonly Serializer serializer;
    private readonly OptionsModel options;

    public ImageService(Serializer serializer, OptionsModel options)
    {
        this.serializer = serializer;
        this.options = options;
    }

    public async Task<Stream?> GetAsync(string id)
    {
        var path = GetPath(id);
        if (File.Exists(path))
        {
            return File.OpenRead(path);
        }
        return null;
    }

    public async Task<string> NewAsync(Stream stream)
    {
        var id = Common.GenerateId();
        var image = await Image.LoadAsync(stream);
        var path = GetPath(id);
        await image.SaveAsJpegAsync(path, new JpegEncoder() { Quality = 97 });

        var imageModel = await serializer.ReadAsync<ImagesModel>() ?? new();
        imageModel.Images.Add(new()
        {
            Id = id
        });
        await serializer.WriteAsync(imageModel);
        return id;
    }

    public async Task<string> PutAsync(string id, string refId)
    {
        var imageModel = await serializer.ReadAsync<ImagesModel>() ?? new();
        imageModel.Images.SingleOrDefault(info => info.Id == id)?.Refs?.Add(refId);
        await serializer.WriteAsync(imageModel);

        return id;
    }

    public async Task DeleteAsync(string id, string refId)
    {
        var imageModel = await serializer.ReadAsync<ImagesModel>() ?? new();
        imageModel.Images.SingleOrDefault(info => info.Id == id)?.Refs?.Remove(refId);
        if ((Random.Shared.Next() % 2) == 0)
        {
            foreach (var item in imageModel.Images.Where(info => info.Refs.Count <= 0))
            {
                var path = GetPath(item.Id);
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            imageModel.Images.RemoveAll(info => info.Refs.Count <= 0);
        }
        await serializer.WriteAsync(imageModel);
    }

    private string GetPath(string id)
    {
        return Path.Combine(options.DataLocation, $"image-{id}.jpg");
    }
}
