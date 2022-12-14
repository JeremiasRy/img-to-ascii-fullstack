using System.Drawing;
using System.Runtime.Versioning;

namespace ImgToASCII;

public class ImgToASCIIConverter
{
    readonly Bitmap _image;
    readonly float _originalHeight;
    readonly float _originalWidth;
    readonly float _expectedHeight;
    readonly float _expectedWidth;
    readonly string _grayScale = "@%#*+=-:. ";
    readonly float _grayScaleMultiplier;
    readonly int _intervalToRemoveRow = 3;
    readonly List<string> _rows = new();

    [SupportedOSPlatform("windows")]
    void Rows()
    {
        bool applyGreynessFromPreviousRow = false;
        for (int iY = 0; iY < _originalHeight; iY++)
        {
            string row = "";
            if (iY % _intervalToRemoveRow == 0)
            { 
                applyGreynessFromPreviousRow = true;
                continue;
            }
            for (int iX = 0; iX < _originalWidth; iX++)
            {
                Color pixel = _image.GetPixel(iX, iY);
                float pixelGrayScale = (pixel.R + pixel.G + pixel.B) / 3;
                if (applyGreynessFromPreviousRow)
                {
                    Color previousRowPixel = _image.GetPixel(iX, iY - 1);
                    float previousPixelGrayScale = (previousRowPixel.R + previousRowPixel.G + previousRowPixel.B) / 3;
                    pixelGrayScale = (pixelGrayScale + previousPixelGrayScale) / 2;
                    applyGreynessFromPreviousRow = false;
                }
                row += _grayScale.ElementAt((int)Math.Round(_grayScaleMultiplier * pixelGrayScale));   
            }
            _rows.Add(row);
        }
    }
    [SupportedOSPlatform("windows")]
    public async Task<AsciiPicture> GetAsciiPicture()
    {
        await Task.Run(() => Rows());
        return new AsciiPicture(_rows.ToArray());
    }
    public ImgToASCIIConverter(Stream image, int expectedWidth, int expectedHeight)
    {
        if (!OperatingSystem.IsWindows())
            throw new Exception("Operating system is not windows.. sorry");

        if (Image.FromStream(image) is not Bitmap bitmap)
            throw new ArgumentException("Image format is malform");

        _image = bitmap;
        _originalHeight = _image.Height;
        _originalWidth = _image.Width;
        _expectedHeight = expectedHeight;
        _expectedWidth = expectedWidth;

        _grayScaleMultiplier = (_grayScale.Length - 1) / (float)255;
    }
}
