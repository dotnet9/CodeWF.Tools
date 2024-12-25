using CodeWF.Tools.FileExtensions;
using CodeWF.Tools.Helpers;

namespace CodeWF.Tools.Test;

public class ImageHelperTest
{
    [Fact]
    public async Task Test_MergeGenerateIcon_Success()
    {
        GetImageInfo(out var sourceImage, out var destImage);
        await ImageHelper.MergeGenerateIcon(sourceImage, destImage, [24, 32]);

        Assert.True(File.Exists(destImage));

        FileHelper.DeleteFileIfExist(destImage);
    }

    [Fact]
    public async Task Test_SeparateGenerateIcon_Success()
    {
        GetImageInfo(out var sourceImage, out var destImage);
        await ImageHelper.SeparateGenerateIcon(sourceImage, destImage, [24, 32]);

        var destDir = Path.GetDirectoryName(destImage);
        var iconFiles = Directory.GetFiles(destDir, "*.ico");
        Assert.True(iconFiles.Length > 0);

        foreach (var iconFile in iconFiles)
        {
            FileHelper.DeleteFileIfExist(iconFile);
        }
    }

    private void GetImageInfo(out string sourceImage, out string destImage)
    {
        sourceImage = "../../logo.png";
        Assert.True(File.Exists(sourceImage));

        destImage = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logo.ico");
        Assert.False(File.Exists(destImage));
    }
}