using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImgToASCII;

public class AsciiPicture
{
    public int Width { get; set; }
    public int Height { get; set; }

    public string[]? Rows { get; set; }
    public AsciiPicture(string[] rows)
    {
        var firstRow = rows.FirstOrDefault();
        if (firstRow is null)
            return;
        Width = firstRow.Length;
        Height = rows.Length;
        Rows = rows;
    }
}
