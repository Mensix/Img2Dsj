using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Xml;
using Img2Dsj;
using MoreLinq;

Settings settings = JsonSerializer.Deserialize<Settings>(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "settings.json")));
Color colorToIgnore = new();
if (settings.ColorToIgnore != null)
{
    colorToIgnore = ColorTranslator.FromHtml(settings.ColorToIgnore);
}
XmlWriter xmlWriter = XmlWriter.Create("output.txt", new()
{
    Indent = true,
    OmitXmlDeclaration = true
});

using (Bitmap initialBitmap = (Bitmap)Image.FromFile(Path.Combine(Directory.GetCurrentDirectory(), settings.FileName)))
using (Bitmap bitmap = BitmapUtils.ResizeImage(initialBitmap, initialBitmap.Width / settings.ScalingFactor, initialBitmap.Height / settings.ScalingFactor))
{
    double x0 = -bitmap.Width / (2 / settings.PixelSize);
    double y0 = Math.Abs((bitmap.Height / (2 / settings.PixelSize)) - settings.OriginDistance);

    List<List<string>> initialHexes = new();
    for (int x = 0; x < bitmap.Height; x++)
    {
        initialHexes.Add(new List<string>());
        for (int y = 0; y < bitmap.Width; y++)
        {
            Color currentColor = bitmap.GetPixel(y, x);
            if (colorToIgnore != default)
            {
                if (currentColor.R != colorToIgnore.R && currentColor.G != colorToIgnore.G && currentColor.B != colorToIgnore.B)
                {
                    initialHexes[x].Add($"0x{currentColor.R:X2}{currentColor.G:X2}{currentColor.B:X2}");
                }
                else
                {
                    initialHexes[x].Add(null);
                }
            }
            else
            {
                initialHexes[x].Add($"0x{currentColor.R:X2}{currentColor.G:X2}{currentColor.B:X2}");
            }
        }
    }

    List<List<List<string>>> hexes = new();
    foreach (List<string> x in initialHexes)
    {
        hexes.Add(x.GroupAdjacent(x => x).Select(x => x.ToList()).ToList());
    }

    xmlWriter.WriteStartElement("custom-markings");
    xmlWriter.WriteStartElement("summer");

    for (int x = 0; x < hexes.Count; x++)
    {
        string _y0 = y0.ToString(CultureInfo.InvariantCulture);
        for (int y = 0; y < hexes[x].Count; y++)
        {
            if (hexes[x][y].All(x => x == null))
            {
                x0 += settings.PixelSize * hexes[x][y].Count;
                continue;
            }

            xmlWriter.WriteStartElement("line");
            xmlWriter.WriteAttributeString("d", _y0);
            xmlWriter.WriteAttributeString("z1", x0.ToString(CultureInfo.InvariantCulture));
            xmlWriter.WriteAttributeString("z2", (x0 + (settings.PixelSize * hexes[x][y].Count)).ToString(CultureInfo.InvariantCulture));
            xmlWriter.WriteAttributeString("c", hexes[x][y][0]);
            xmlWriter.WriteAttributeString("w", settings.PixelSize.ToString(CultureInfo.InvariantCulture));
            xmlWriter.WriteEndElement();
            x0 += settings.PixelSize * hexes[x][y].Count;
        }

        x0 = -bitmap.Width / (2 / settings.PixelSize);
        y0 += settings.PixelSize;
    }

    xmlWriter.WriteEndElement();
    xmlWriter.WriteEndElement();
    xmlWriter.Close();
}
