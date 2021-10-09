using System;
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
            List<List<string>> initialPixels = BitmapUtils.ParsePixels(bitmap, settings);
            Marking marking = new()
            {
                Summer = settings.TagsToInclude.Any(x => x == "banner" || x == "line")
                    ? new()
                    {
                        Banners = new List<Banner>(),
                        Lines = new List<Line>()
                    }
                    : null,
                Winter = settings.TagsToInclude.Any(x => x == "spray" || x == "twigs")
                    ? new()
                    {
                        Sprays = settings.TagsToInclude.Contains("spray") ? new List<Spray>() : null,
                        Twigs = settings.TagsToInclude.Contains("twigs") ? new List<Twigs>() : null
                    }
                    : null
            };

            XmlWriterSettings xmlWriterSettings = new()
            {
                OmitXmlDeclaration = true
            };
            XmlSerializerNamespaces xmlSerializerNamespaces = new(new[] { XmlQualifiedName.Empty });
            XmlSerializer xmlSerializer = new(typeof(Marking));

            (double x0, double y0) = BitmapUtils.GetOriginCoordinates(bitmap, settings);
            List<List<List<string>>> pixels = initialPixels.MergeSamePixels();
            List<List<List<string>>> monocoloredPixels = BitmapUtils.ParseMonocolorPixels(initialPixels, settings).MergeSamePixels();
            List<List<List<string>>> pixelsToUse = settings.ColorToUse != null || settings.TagsToInclude.Contains("twigs") ? monocoloredPixels : pixels;

            for (int i = 0; i < pixelsToUse.Count; i++)
            {
                for (int j = 0; j < pixelsToUse[i].Count; j++)
                {
                    if (pixelsToUse[i][j].All(x => x == null))
                    {
                        x0 += settings.PixelSize * pixelsToUse[i][j].Count;
                        continue;
                    }

                    if (settings.TagsToInclude.Contains("banner"))
                    {
                        marking.Summer.Banners.Add(new Banner
                        {
                            D1 = Math.Round(y0, 2),
                            D2 = Math.Round(y0 + settings.PixelSize, 2),
                            Z1 = Math.Round(x0, 2),
                            Z2 = Math.Round(x0 + (settings.PixelSize * pixelsToUse[i][j].Count), 2),
                            C = pixelsToUse[i][j][0],
                            W = settings.PixelSize
                        });
                    }
                    if (settings.TagsToInclude.Contains("line"))
                    {
                        marking.Summer.Lines.Add(new Line
                        {
                            D = Math.Round(y0, 2),
                            Z1 = Math.Round(x0, 2),
                            Z2 = Math.Round(x0 + (settings.PixelSize * pixelsToUse[i][j].Count), 2),
                            C = pixelsToUse[i][j][0],
                            W = settings.PixelSize
                        });
                    }
                    if (settings.TagsToInclude.Contains("spray"))
                    {
                        marking.Winter.Sprays.Add(new Spray
                        {
                            D = Math.Round(y0, 2),
                            Z1 = Math.Round(x0 + settings.OriginDistance.Z, 2),
                            Z2 = Math.Round(x0 + settings.OriginDistance.Z + (settings.PixelSize * pixelsToUse[i][j].Count), 2),
                            C = pixelsToUse[i][j][0],
                            W = Math.Round(settings.PixelSize * 3, 2)
                        });
                    }
                    if (settings.TagsToInclude.Contains("twigs"))
                    {
                        marking.Winter.Twigs.Add(new Twigs
                        {
                            D = Math.Round(y0, 2),
                            Z1 = Math.Round(x0 + settings.OriginDistance.Z, 2),
                            Z2 = Math.Round(x0 + settings.OriginDistance.Z + (settings.PixelSize * pixelsToUse[i][j].Count), 2),
                        });
                    }
                    x0 += settings.PixelSize * pixelsToUse[i][j].Count;
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