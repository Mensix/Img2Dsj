using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using Img2Dsj.Models;

namespace Img2Dsj.Utils
{
    public static class XmlUtils
    {
        public static void GenerateMarkings(Bitmap bitmap, Settings settings)
        {
            Marking marking = new()
            {
                Summer = new()
                {
                    Lines = new List<Line>(),
                },
                Winter = new()
                {
                    Sprays = new List<Spray>()
                }
            };

            XmlWriterSettings xmlWriterSettings = new()
            {
                OmitXmlDeclaration = true
            };
            XmlSerializerNamespaces xmlSerializerNamespaces = new(new[] { XmlQualifiedName.Empty });
            XmlSerializer xmlSerializer = new(typeof(Marking));

            (double x0, double y0) = BitmapUtils.GetOriginCoordinates(bitmap, settings);
            List<List<List<string>>> pixels = BitmapUtils.ParsePixels(bitmap, settings);

            for (int i = 0; i < pixels.Count; i++)
            {
                for (int j = 0; j < pixels[i].Count; j++)
                {
                    if (pixels[i][j].All(x => x == null))
                    {
                        x0 += settings.PixelSize * pixels[i][j].Count;
                        continue;
                    }

                    marking.Summer.Lines.Add(new Line
                    {
                        D = y0,
                        Z1 = x0,
                        Z2 = x0 + (settings.PixelSize * pixels[i][j].Count),
                        C = pixels[i][j][0],
                        W = settings.PixelSize
                    });
                    marking.Winter.Sprays.Add(new Spray
                    {
                        D = y0,
                        Z1 = x0,
                        Z2 = x0 + (settings.PixelSize * pixels[i][j].Count),
                        C = pixels[i][j][0],
                        W = settings.PixelSize
                    });
                    x0 += settings.PixelSize * pixels[i][j].Count;
                }

                x0 = -bitmap.Width / (2 / settings.PixelSize);
                y0 += settings.PixelSize;
            }

            using StreamWriter streamWriter = new("output.txt");
            using XmlWriter xmlWriter = XmlWriter.Create(streamWriter, xmlWriterSettings);
            xmlSerializer.Serialize(streamWriter, marking, xmlSerializerNamespaces);
        }
    }
}