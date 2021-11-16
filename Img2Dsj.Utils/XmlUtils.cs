using System.Diagnostics.CodeAnalysis;
using System.Xml;
using System.Xml.Serialization;
using Img2Dsj.Models;
using SkiaSharp;

namespace Img2Dsj.Utils
{
    public static class XmlUtils
    {
        public static void GenerateMarkings(SKBitmap bitmap, Settings settings)
        {
            XmlWriterSettings xmlWriterSettings = new()
            {
                OmitXmlDeclaration = true,
                Indent = true,
                NewLineChars = "\r\n",
                NewLineHandling = NewLineHandling.Replace
            };
            XmlSerializerNamespaces xmlSerializerNamespaces = new(new[] { XmlQualifiedName.Empty });
            XmlSerializer xmlSerializer = new(typeof(Marking));

            List<List<string>> initialPixels = BitmapUtils.ParsePixels(bitmap, settings);
            Marking marking = new()
            {
                Summer = settings.TagsToInclude.Any(x => x is "banner" or "line")
                    ? new()
                    {
                        Banners = new List<Banner>(),
                        Lines = new List<Line>()
                    }
                    : null,
                Winter = settings.TagsToInclude.Any(x => x is "spray" or "twigs")
                    ? new()
                    {
                        Sprays = new List<Spray>(),
                        Twigs = new List<Twigs>()
                    }
                    : null
            };

            (double x0, double y0) = BitmapUtils.GetOriginCoordinates(bitmap, settings);
            List<List<List<string>>> pixels = initialPixels.MergeSamePixels();
            List<List<List<string>>> monocoloredPixels = initialPixels.ToMonocolorPixels(settings).MergeSamePixels();

            if (settings.TagsToInclude.Contains("twigs"))
            {
                for (int i = 0; i < monocoloredPixels.Count; i++)
                {
                    for (int j = 0; j < monocoloredPixels[i].Count; j++)
                    {
                        if (monocoloredPixels[i][j].All(x => x == null))
                        {
                            x0 += settings.PixelSize * monocoloredPixels[i][j].Count;
                            continue;
                        }

                        marking.Winter.Twigs.Add(new Twigs
                        {
                            D = Math.Round(y0, 3, MidpointRounding.ToZero),
                            Z1 = Math.Round(x0 + settings.OriginDistance.Z, 3, MidpointRounding.ToZero),
                            Z2 = Math.Round(x0 + settings.OriginDistance.Z + (settings.PixelSize * monocoloredPixels[i][j].Count), 3, MidpointRounding.ToZero),
                        });
                        x0 += settings.PixelSize * monocoloredPixels[i][j].Count;
                    }

                    x0 = -bitmap.Width / (2 / settings.PixelSize);
                    y0 += settings.PixelSize;
                }
            }
            if (settings.TagsToInclude.Any(x => x is "banner" or "spray" or "line"))
            {
                if (settings.ColorToUse != null)
                {
                    pixels = monocoloredPixels;
                }

                for (int i = 0; i < pixels.Count; i++)
                {
                    for (int j = 0; j < pixels[i].Count; j++)
                    {
                        if (pixels[i][j].All(x => x == null))
                        {
                            x0 += settings.PixelSize * pixels[i][j].Count;
                            continue;
                        }

                        if (settings.TagsToInclude.Contains("banner"))
                        {
                            marking.Summer.Banners.Add(new Banner
                            {
                                D1 = Math.Round(y0, 3, MidpointRounding.ToZero),
                                D2 = Math.Round(y0 + settings.PixelSize, 3, MidpointRounding.ToZero),
                                Z1 = Math.Round(x0, 3, MidpointRounding.ToZero),
                                Z2 = Math.Round(x0 + (settings.PixelSize * pixels[i][j].Count), 3, MidpointRounding.ToZero),
                                C = pixels[i][j][0],
                                W = settings.PixelSize
                            });
                        }
                        if (settings.TagsToInclude.Contains("line"))
                        {
                            marking.Summer.Lines.Add(new Line
                            {
                                D = Math.Round(y0, 3, MidpointRounding.ToZero),
                                Z1 = Math.Round(x0, 3, MidpointRounding.ToZero),
                                Z2 = Math.Round(x0 + (settings.PixelSize * pixels[i][j].Count), 3, MidpointRounding.ToZero),
                                C = pixels[i][j][0],
                                W = settings.PixelSize
                            });
                        }
                        if (settings.TagsToInclude.Contains("spray"))
                        {
                            marking.Winter.Sprays.Add(new Spray
                            {
                                D = Math.Round(y0, 3, MidpointRounding.ToZero),
                                Z1 = Math.Round(x0 + settings.OriginDistance.Z, 3, MidpointRounding.ToZero),
                                Z2 = Math.Round(x0 + settings.OriginDistance.Z + (settings.PixelSize * pixels[i][j].Count), 3, MidpointRounding.ToZero),
                                C = pixels[i][j][0],
                                W = Math.Round(settings.PixelSize * 3, 3, MidpointRounding.ToZero)
                            });
                        }

                        x0 += settings.PixelSize * pixels[i][j].Count;
                    }

                    x0 = (-bitmap.Width / (2 / settings.PixelSize)) + settings.OriginDistance.Z;
                    y0 += settings.PixelSize;
                }
            }

            using StreamWriter streamWriter = new("output.xml");
            using XmlWriter xmlWriter = XmlWriter.Create(streamWriter, xmlWriterSettings);
            xmlSerializer.Serialize(streamWriter, marking, xmlSerializerNamespaces);
        }
    }
}