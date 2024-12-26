using ImageMagick;
using System.IO;
using System.Threading.Tasks;

namespace CodeWF.Tools.Helpers;

public static class ImageHelper
{
    public static async Task MergeGenerateIcon(string sourceImagePath, string destIconPath, uint[] sizes)
    {
        var baseImage = new MagickImage(sourceImagePath);
        var collection = new MagickImageCollection();

        foreach (var size in sizes)
        {
            var resizedImage = baseImage.Clone();
            resizedImage.Resize(size, size);
            collection.Add(resizedImage);
        }

        await collection.WriteAsync(destIconPath);
    }

    public static async Task SeparateGenerateIcon(string sourceImagePath, string destIconFolder, uint[] sizes)
    {
        var fileName = Path.GetFileNameWithoutExtension(sourceImagePath);

        var baseImage = new MagickImage(sourceImagePath);

        foreach (var size in sizes)
        {
            var resizedImage = baseImage.Clone();
            resizedImage.Resize(size, size);

            var savePath = Path.Combine(destIconFolder, $"{fileName}-{size}x{size}.ico");
            await resizedImage.WriteAsync(savePath);
        }
    }
}