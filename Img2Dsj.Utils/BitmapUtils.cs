using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Linq;
using Img2Dsj.Models;

namespace Img2Dsj.Utils
{
    public static class BitmapUtils
    {
        public static Bitmap ResizeImage(Image image, double width, double height)
        {
            int fixedWidth = Convert.ToInt32(width);
            int fixedHeight = Convert.ToInt32(height);

            Rectangle rectangle = new(0, 0, fixedWidth, fixedHeight);
            Bitmap bitmap = new(fixedWidth, fixedHeight);
            bitmap.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using ImageAttributes imageAttributes = new();
                imageAttributes.SetWrapMode(WrapMode.TileFlipXY);
                graphics.DrawImage(image, rectangle, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
            }

            return bitmap;
        }

        public static List<List<string>> ParsePixels(Bitmap bitmap, Settings settings)
        {
            List<List<string>> initialPixels = new();
            List<Color> colorsToIgnore = settings.ColorsToIgnore.Select(x => ColorTranslator.FromHtml(x)).ToList();

            for (int x = 0; x < bitmap.Height; x++)
            {
                initialPixels.Add(new List<string>());
                for (int y = 0; y < bitmap.Width; y++)
                {
                    Color currentColor = bitmap.GetPixel(y, x);
                    if (colorsToIgnore != default)
                    {
                        if (!colorsToIgnore.Select(x => x.ToArgb()).Any(x => x.Equals(currentColor.ToArgb())))
                        {
                            initialPixels[x].Add($"0x{currentColor.R:X2}{currentColor.G:X2}{currentColor.B:X2}");
                        }
                        else
                        {
                            initialPixels[x].Add(null);
                        }
                    }
                    else
                    {
                        initialPixels[x].Add($"0x{currentColor.R:X2}{currentColor.G:X2}{currentColor.B:X2}");
                    }
                }
            }

            return initialPixels;
        }

        public static List<List<string>> ParseMonocolorPixels(List<List<string>> initialPixels, Settings settings)
        {
            List<List<string>> monocolorPixels = new();
            foreach (List<string> pixels in initialPixels)
            {
                monocolorPixels.Add(pixels.ConvertAll(x =>
                {
                    if (x != null)
                    {
                        x = settings.ColorToUse ?? "0x000000";
                    }
                    return x;
                }));
            }

            return monocolorPixels;
        }

        public static (double, double) GetOriginCoordinates(Bitmap bitmap, Settings settings)
        {
            return (-bitmap.Width / (2 / settings.PixelSize), Math.Abs((bitmap.Height / (2 / settings.PixelSize)) - settings.OriginDistance.X));
        }

        private static FontStyle MapFontStyle(this Settings settings)
        {
            return settings.TextToDraw.Style switch
            {
                "bold" => FontStyle.Bold,
                "italic" => FontStyle.Italic,
                "strikeout" => FontStyle.Strikeout,
                "underline" => FontStyle.Underline,
                _ => FontStyle.Regular,
            };
        }
        public static Bitmap DrawTextImage(Settings settings)
        {
            SizeF textSize;
            Font font = new(settings.TextToDraw.Font, settings.TextToDraw.Size, settings.MapFontStyle());
            using (Image image = new Bitmap(1, 1))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                textSize = graphics.MeasureString(settings.TextToDraw.Text, font);
            }

            Image _ = new Bitmap((int)textSize.Width, (int)textSize.Height);
            using (Graphics graphics = Graphics.FromImage(_))
            {
                graphics.Clear(Color.Black);
                using Brush brush = new SolidBrush(ColorTranslator.FromHtml(settings.TextToDraw.Color));
                graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
                graphics.DrawString(settings.TextToDraw.Text, font, brush, 0, 0);
                graphics.Save();
            }

            return (Bitmap)_;
        }
    }
}