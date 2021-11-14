using Img2Dsj.Models;
using SkiaSharp;

namespace Img2Dsj.Utils
{
    public static class BitmapUtils
    {
        public static List<List<string>> ParsePixels(SKBitmap bitmap, Settings settings)
        {
            List<List<string>> pixels = new();
            using (SKBitmap _ = SKBitmap.Decode(settings.FileName))
            {
                pixels = bitmap.Pixels.Select(x => settings.ColorsToIgnore?.Any(y => x.ToString() == y) == true || x.Alpha == 0 ? null : x.ToString().Replace("#", "0x")[..8]).Chunk(bitmap.Width).Select(x => x.ToList()).ToList();
            }

            return pixels;
        }

        public static (double, double) GetOriginCoordinates(SKBitmap bitmap, Settings settings)
        {
            return ((-bitmap.Width / (2 / settings.PixelSize)) + settings.OriginDistance.Z, Math.Abs((bitmap.Height / (2 / settings.PixelSize)) - settings.OriginDistance.X));
        }
    }
}