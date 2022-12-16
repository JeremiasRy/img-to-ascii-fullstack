using System.Drawing;
using System.Runtime.Versioning;

namespace ImgToASCII;

public class ImgToASCIIConverter
{
    readonly Bitmap _image;
    readonly float _originalHeight;
    readonly float _originalWidth;
    readonly float _maxOutputHeight;
    readonly float _maxOutputWidth;
    readonly string _grayScale = "@%#*+=-:. ";
    readonly float _grayScaleMultiplier;
    readonly int _intervalToRemoveRow = 3;
    float _imageRatioWidthToHeight;
    float _imageRatioHeightToWidth;
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
        _imageRatioWidthToHeight = _originalWidth / _rows.Count;
        _imageRatioHeightToWidth = _rows.Count / _originalWidth;
        return Task.CompletedTask;
    }

    Task FormatRows()
    {

        float scaleToApplyWidth = _maxOutputWidth / _originalWidth;
        float scaleToApplyHeight = _maxOutputHeight / _rows.Count;
        if (scaleToApplyWidth > 1 || scaleToApplyHeight > 1)
        {
            return Task.CompletedTask;
        }
        float newWidth = 0;
        float newHeight = 0;
        if (scaleToApplyWidth < scaleToApplyHeight)
        {
            newWidth = _maxOutputWidth;
        } else
        {
            newHeight = _maxOutputHeight;
        }
        
        if (newWidth == 0)
        {
            newWidth = (float)Math.Floor(newHeight * _imageRatioWidthToHeight);
        } else if (newHeight == 0)
        {
            newHeight = (float)Math.Floor(newWidth * _imageRatioHeightToWidth);
        }
        
        int pixelSizeWidth = (int)Math.Round(_originalWidth / newWidth);
        int pixelSizeHeigth = (int)Math.Round(_rows.Count / newHeight);
        

        List<string> adjustedRows = new();
        List<string> adjustedPicture = new();
        for (int i = 0; i < newHeight; i++)
        {
            adjustedPicture.Add("");
        }

        for (int i = 0; i < _rows.Count; i++) //Adjust rows
        {
            string compiledRow = "";
            var chars = SplitIntoChunks(_rows[i], pixelSizeWidth).Select(str => AdjustedChar(str));
            foreach (char ch in chars)
            {
                compiledRow += ch;
            };
            var length = compiledRow.Length;
            adjustedRows.Add(compiledRow);
        };

        for (int i = 0; i < adjustedRows.First().Length; i++) //Adjust columns
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
        _maxOutputHeight = expectedHeight;
        _maxOutputWidth = expectedWidth;

        _grayScaleMultiplier = (_grayScale.Length - 1) / (float)255;
    }
}
