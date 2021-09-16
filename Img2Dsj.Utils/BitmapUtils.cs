using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Img2Dsj.Models;

namespace Img2Dsj.Utils
{
    public static class BitmapUtils
    {
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            Rectangle rectangle = new(0, 0, width, height);
            Bitmap bitmap = new(width, height);
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

        public static List<List<List<string>>> ParsePixels(Bitmap bitmap, Settings settings)
        {
            List<List<string>> initialPixels = new();
            Color colorToIgnore = ColorTranslator.FromHtml(settings.ColorToIgnore);

            for (int x = 0; x < bitmap.Height; x++)
            {
                initialPixels.Add(new List<string>());
                for (int y = 0; y < bitmap.Width; y++)
                {
                    Color currentColor = bitmap.GetPixel(y, x);
                    if (colorToIgnore != default)
                    {
                        if (!colorToIgnore.ToArgb().Equals(currentColor.ToArgb()))
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

            return PixelUtils.MergeSamePixels(initialPixels);
        }

        public static (double, double) GetOriginCoordinates(Bitmap bitmap, Settings settings)
        {
            return (-bitmap.Width / (2 / settings.PixelSize), Math.Abs((bitmap.Height / (2 / settings.PixelSize)) - settings.OriginDistance));
        }
    }
}