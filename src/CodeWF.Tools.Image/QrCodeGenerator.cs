using ImageMagick;
using ImageMagick.Drawing;
using ZXing;
using ZXing.QrCode;

namespace CodeWF.Tools.Image;

public static class QrCodeGenerator
{
    public static void GenerateQrCode(string phoneNumber, string promptText, string imagePath)
    {
        // 生成二维码
        var qrCodeWriter = new BarcodeWriterPixelData
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions
            {
                Width = 250,                // 增加尺寸以提高清晰度
                Height = 250,               // 增加尺寸以提高清晰度
                Margin = 1,                 // 设置合适的边距
                ErrorCorrection = ZXing.QrCode.Internal.ErrorCorrectionLevel.M,  // 使用中等纠错级别
                CharacterSet = "UTF-8",
                DisableECI = true
            }
        };

        var content = $"tel:{phoneNumber}";
        var pixelData = qrCodeWriter.Write(content);

        // 创建二维码图像
        using var qrCodeImage = new MagickImage();
        var settings = new PixelReadSettings((uint)pixelData.Width, (uint)pixelData.Height, StorageType.Char, PixelMapping.RGBA);
        qrCodeImage.ReadPixels(pixelData.Pixels, settings);

        // 创建白色背景
        using var background = new MagickImage(MagickColors.White, 600, 300);

        // 添加文字
        var wordImage = new Drawables()
            .Font("KaiTi") // 添加支持中文的字体
            .FontPointSize(32)
            .FillColor(MagickColors.Blue) // 设置文字颜色
            .TextAlignment(TextAlignment.Left) // 左对齐
            .Text(25, 150, promptText)
            .FontPointSize(14)
            .FillColor(MagickColors.Black) // 设置文字颜色
            .Text(50, 250, "生成网址：https://dotnet9.com/qrcode");
        background.Draw(wordImage);

        // 合并二维码到背景
        background.Composite(qrCodeImage, 325, 25, CompositeOperator.Over);

        // 保存图像
        background.Write(imagePath);
    }
}