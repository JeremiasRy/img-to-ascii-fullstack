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
    List<string> _rows = new();

    [SupportedOSPlatform("windows")]
    Task Rows()
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
        return Task.CompletedTask;
    }

    Task FormatRows()
    {
        float scaleToApplyWidth = _expectedWidth / _originalWidth;
        float scaleToApplyHeight = _expectedHeight / _rows.Count;
        int pixelSizeWidth = (int)Math.Floor(1 / scaleToApplyWidth);
        int pixelSizeHeigth = (int)Math.Floor(1 / scaleToApplyHeight);
        int newWidth;
        int newHeight = _rows.Count / pixelSizeHeigth;

        List<string> adjustedRows = new();
        List<string> adjustedPicture = new();
        for (int i = 0; i < newHeight; i++)
        {
            adjustedPicture.Add("");
        }

        for (int i = 0; i < _rows.Count; i++) //Adjust columns
        {
            string compiledRow = "";
            var chars = SplitIntoChunks(_rows[i], pixelSizeWidth).Select(str => AdjustedChar(str));
            foreach (char ch in chars)
            {
                compiledRow += ch;
            };
            adjustedRows.Add(compiledRow);
        };
        newWidth = adjustedRows.First().Length;

        for (int i = 0; i < newWidth; i++) //Adjust rows
        {
            string rowString = "";
            foreach (var row in adjustedRows)
            {
                rowString += row[i];
            }
            var chars = SplitIntoChunks(rowString, pixelSizeHeigth).Select(str => AdjustedChar(str));
            string formattedRow = "";
            foreach(char ch in chars)
            {
                formattedRow += ch;
            }
            for (int j = 0; j < formattedRow.Length; j++)
            {
                adjustedPicture[j] += formattedRow.ElementAt(j);
            }
        }
        _rows = adjustedPicture;
        return Task.CompletedTask;
    }

    char AdjustedChar(string stringToSquash)
    {
        var indexOfNewChar = (int)Math.Round(stringToSquash.ToCharArray().Select(character => _grayScale.IndexOf(character)).Average());
        return _grayScale.ElementAt(indexOfNewChar);
    }

    static IEnumerable<string> SplitIntoChunks(string str, int chunkSize)
    {
        return Enumerable.Range(0, str.Length / chunkSize)
            .Select(i => str.Substring(i * chunkSize, chunkSize));
    }
    [SupportedOSPlatform("windows")]
    public async Task<AsciiPicture> GetAsciiPicture()
    {
        await Rows();
        await FormatRows();
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
