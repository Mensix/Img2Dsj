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
            Marking marking = new();

            if (settings.TagsToInclude.Contains("line"))
            {
                marking.Summer = new()
                {
                    Lines = new List<Line>()
                };
            }

            if (settings.TagsToInclude.Contains("spray"))
            {
                marking.Winter = new()
                {
                    Sprays = new List<Spray>()
                };
            }
            else if (settings.TagsToInclude.Contains("twigs"))
            {
                marking.Winter = new()
                {
                    Twigs = new List<Twigs>()
                };
            }

            XmlWriterSettings xmlWriterSettings = new()
            {
                OmitXmlDeclaration = true
            };
            XmlSerializerNamespaces xmlSerializerNamespaces = new(new[] { XmlQualifiedName.Empty });
            XmlSerializer xmlSerializer = new(typeof(Marking));

            (double x0, double y0) = BitmapUtils.GetOriginCoordinates(bitmap, settings);
            List<List<List<string>>> pixels = initialPixels.MergeSamePixels();
            List<List<List<string>>> monocoloredPixels = BitmapUtils.ParseMonocolorPixels(initialPixels).MergeSamePixels();

            if (settings.TagsToInclude.Contains("line") || settings.TagsToInclude.Contains("spray"))
            {
                for (int i = 0; i < pixels.Count; i++)
                {
                    for (int j = 0; j < pixels[i].Count; j++)
                    {
                        if (pixels[i][j].All(x => x == null))
                        {
                            x0 += settings.PixelSize * pixels[i][j].Count;
                            continue;
                        }

                        if (settings.TagsToInclude.Contains("line"))
                        {
                            marking.Summer.Lines.Add(new Line
                            {
                                D = Math.Round(y0, 2),
                                Z1 = Math.Round(x0, 2),
                                Z2 = Math.Round(x0 + (settings.PixelSize * pixels[i][j].Count), 2),
                                C = pixels[i][j][0],
                                W = settings.PixelSize
                            });
                        }
                        if (settings.TagsToInclude.Contains("spray"))
                        {
                            marking.Winter.Sprays.Add(new Spray
                            {
                                D = Math.Round(y0, 2),
                                Z1 = Math.Round(x0 + settings.OriginDistance.Z, 2),
                                Z2 = Math.Round(x0 + settings.OriginDistance.Z + (settings.PixelSize * pixels[i][j].Count), 2),
                                C = pixels[i][j][0],
                                W = Math.Round(settings.PixelSize * 3, 2)
                            });
                        }
                        x0 += settings.PixelSize * pixels[i][j].Count;
                    }

                    x0 = -bitmap.Width / (2 / settings.PixelSize);
                    y0 += settings.PixelSize;
                }
            }

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
                            D = Math.Round(y0, 2),
                            Z1 = Math.Round(x0 + settings.OriginDistance.Z, 2),
                            Z2 = Math.Round(x0 + settings.OriginDistance.Z + (settings.PixelSize * monocoloredPixels[i][j].Count), 2),
                        });
                        x0 += settings.PixelSize * monocoloredPixels[i][j].Count;
                    }

                    x0 = -bitmap.Width / (2 / settings.PixelSize);
                    y0 += settings.PixelSize;
                }
            }

            using StreamWriter streamWriter = new("output.txt");
            using XmlWriter xmlWriter = XmlWriter.Create(streamWriter, xmlWriterSettings);
            xmlSerializer.Serialize(streamWriter, marking, xmlSerializerNamespaces);
        }
    }
}