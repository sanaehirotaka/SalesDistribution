using Microsoft.AspNetCore.Mvc;
using SalesDistribution.Services;
using SixLabors.ImageSharp;

namespace SalesDistribution.Apis;

[Route("api/[controller]/[action]")]
[ApiController]
public class ImageController : ControllerBase
{
    private readonly Serializer serializer;
    private readonly ImageService imageService;

    public ImageController(Serializer serializer, ImageService imageService)
    {
        this.serializer = serializer;
        this.imageService = imageService;
    }

    [HttpPost]
    [Produces("application/json")]
    public async Task<List<string>> Upload([FromForm] List<IFormFile> Images)
    {
        var result = new List<string>();
        foreach (var image in Images)
        {
            using var stream = image.OpenReadStream();
            result.Add(await imageService.NewAsync(stream));
        }
        return result;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Download(string id)
    {
        var stream = await imageService.GetAsync(id);
        if (stream != null)
        {
            return new FileStreamResult(stream, "image/jpeg");
        }
        return NotFound();
    }
}
