using ImageMagick;
using ImageMagick.Drawing;
using ZXing;
using ZXing.QrCode;

namespace CodeWF.Tools.Image;

public static class QrCodeGenerator
{
    public static void GenerateQrCode(string title, string ad, string content, string imagePath)
    {
        var qrCodeWriter = new BarcodeWriterPixelData
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions
            {
                Width = 360,               
                Height = 360,
                Margin = 2,                 
                ErrorCorrection = ZXing.QrCode.Internal.ErrorCorrectionLevel.H, 
                CharacterSet = "UTF-8",
                DisableECI = true
            }
        };

        var pixelData = qrCodeWriter.Write(content);

        using var qrCodeImage = new MagickImage();
        var settings = new PixelReadSettings((uint)pixelData.Width, (uint)pixelData.Height, StorageType.Char, PixelMapping.RGBA);
        qrCodeImage.ReadPixels(pixelData.Pixels, settings);

        using var background = new MagickImage(MagickColors.White, 360, 420);

        var titleText = new Drawables()
            .Font("KaiTi")
            .FontPointSize(40)
            .FillColor(new MagickColor("#1A237E"))
            .TextAlignment(TextAlignment.Center)
            .Text(180, 45, title);
        background.Draw(titleText);
        
        background.Composite(qrCodeImage, 0, 50, CompositeOperator.Over);

        var adText = new Drawables()
            .Font("KaiTi")
            .FontPointSize(15)
            .FillColor(new MagickColor("#1A237E"))
            .TextAlignment(TextAlignment.Center)
            .Text(180, 400, ad);
        background.Draw(adText);

        background.Quality = 100;
        background.Write(imagePath);
    }
}