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
            XmlWriterSettings xmlWriterSettings = new() { OmitXmlDeclaration = true };
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
                        Sprays = settings.TagsToInclude.Contains("spray") ? new List<Spray>() : null,
                        Twigs = settings.TagsToInclude.Contains("twigs") ? new List<Twigs>() : null
                    }
                    : null
            };

            (double x0, double y0) = BitmapUtils.GetOriginCoordinates(bitmap, settings);
            List<List<List<string>>> pixels = initialPixels.MergeSamePixels();

            if (settings.TagsToInclude.Contains("twigs"))
            {
                List<List<List<string>>> monocoloredPixels = BitmapUtils.ParseMonocolorPixels(initialPixels, settings).MergeSamePixels();
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
                for (int i = 0; i < pixels.Count; i++)
                {
                    for (int j = 0; j < pixels[i].Count; j++)
                    {
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
                    }
                }
            }

            using StreamWriter streamWriter = new("output.xml");
            using XmlWriter xmlWriter = XmlWriter.Create(streamWriter, xmlWriterSettings);
            xmlSerializer.Serialize(streamWriter, marking, xmlSerializerNamespaces);
        }
    }
}