using System.Drawing;
using System.Runtime.Versioning;

namespace ImgToASCII;

public class ImgToASCIIConverter
{
    readonly Bitmap _image;
    readonly float _height;
    readonly float _width;
    readonly string _grayScale = "@%#*+=-:. ";
    readonly float _grayScaleMultiplier;

    [SupportedOSPlatform("windows")]
    public async Task<AsciiPicture> GetAsciiPicture()
    {
        AsciiPicture? asciiPicture = null;
        await Task.Run(() =>
        {
            List<string> rows = new();

            for (int iY = 0; iY < _height; iY++)
            {
                string row = "";
                for (int iX = 0; iX < _width; iX++)
                {
                    Color pixel = _image.GetPixel(iX, iY);
                    float pixelGrayScale = (pixel.R + pixel.G + pixel.B) / 3;
                    row += _grayScale.ElementAt((int)Math.Round(_grayScaleMultiplier * pixelGrayScale));
                }
                rows.Add(row);
            }
            asciiPicture = new AsciiPicture(rows.ToArray());
        });
        return asciiPicture == null ? new AsciiPicture(Array.Empty<string>()) : asciiPicture;
    }
    public ImgToASCIIConverter(Stream image)
    {
        if (!OperatingSystem.IsWindows())
            throw new Exception("Operating system is not windows.. sorry");

        if (Image.FromStream(image) is not Bitmap bitmap)
            throw new ArgumentException("Image format is malform");

        _image = bitmap;
        _height = (float)_image.Height;
        _width = (float)_image.Width;
        _grayScaleMultiplier = (_grayScale.Length - 1) / (float)255;
    }
}
