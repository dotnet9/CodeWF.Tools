using ImageMagick;
using ImageMagick.Drawing;
using ZXing;
using ZXing.QrCode;

namespace CodeWF.Tools.Image;

public static class QrCodeGenerator
{
    public static void GenerateQrCode(string title, string content, string imagePath, string? subTitle = "")
    {
        var qrCodeWriter = new BarcodeWriterPixelData
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions
            {
                Width = 400,
                Height = 400,
                Margin = 1,
                ErrorCorrection = ZXing.QrCode.Internal.ErrorCorrectionLevel.H,
                CharacterSet = "UTF-8",
                DisableECI = true
            }
        };

        var pixelData = qrCodeWriter.Write(content);

        using var qrCodeImage = new MagickImage();
        var settings = new PixelReadSettings((uint)pixelData.Width, (uint)pixelData.Height, StorageType.Char, PixelMapping.RGBA);
        qrCodeImage.ReadPixels(pixelData.Pixels, settings);

        var backgroundHeight = string.IsNullOrWhiteSpace(subTitle) ? 600u : 630u;
        using var background = new MagickImage(MagickColors.White, 500, backgroundHeight);

        background.BorderColor = new MagickColor("#2888E2");
        background.Border(8);

        var titleText = new Drawables()
            .Font("SimHei")
            .FontPointSize(95)
            .FillColor(new MagickColor("#FF5722"))
            .TextAlignment(TextAlignment.Center)
            .Text(250, 120, title);
        background.Draw(titleText);

        background.Composite(qrCodeImage, 50, 170, CompositeOperator.Over);

        if (!string.IsNullOrWhiteSpace(subTitle))
        {
            var subTitleText = new Drawables()
                .Font("SimHei")
                .FontPointSize(20)
                .FillColor(new MagickColor("#333333"))
                .TextAlignment(TextAlignment.Center)
                .Text(250, 600, subTitle);
            background.Draw(subTitleText);
        }

        //using var logo = new MagickImage("logo.png");
        //logo.Resize(100, 100);
        //background.Composite(logo, 250, 250, CompositeOperator.Over);

        background.Quality = 100;
        background.Write(imagePath);
    }
}