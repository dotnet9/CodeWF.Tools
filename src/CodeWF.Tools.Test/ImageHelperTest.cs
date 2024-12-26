using CodeWF.Tools.FileExtensions;
using CodeWF.Tools.Helpers;

namespace CodeWF.Tools.Test;

public class ImageHelperTest
{
    [Fact]
    public async Task Test_MergeGenerateIcon_Success()
    {
        var sourceImage = "../../logo.png";
        Assert.True(File.Exists(sourceImage));

        var destIconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logo.ico");
        Assert.False(File.Exists(destIconPath));

        await ImageHelper.MergeGenerateIcon(sourceImage, destIconPath, [24, 32]);

        Assert.True(File.Exists(destIconPath));

        FileHelper.DeleteFileIfExist(destIconPath);
    }

    [Fact]
    public async Task Test_SeparateGenerateIcon_Success()
    {
        var sourceImage = "../../logo.png";
        Assert.True(File.Exists(sourceImage));

        var destIconFolder = AppDomain.CurrentDomain.BaseDirectory;

        await ImageHelper.SeparateGenerateIcon(sourceImage, destIconFolder, [24, 32]);

        var iconFiles = Directory.GetFiles(destIconFolder, "*.ico");
        Assert.True(iconFiles.Length > 0);

        foreach (var iconFile in iconFiles)
        {
            FileHelper.DeleteFileIfExist(iconFile);
        }
    }
}