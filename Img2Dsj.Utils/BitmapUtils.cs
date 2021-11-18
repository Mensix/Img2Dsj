using Img2Dsj.Models;
using SkiaSharp;
using Topten.RichTextKit;

namespace Img2Dsj.Utils
{
    public static class BitmapUtils
    {
        public static void DrawText(Settings settings)
        {
            RichString richString = new();
            foreach (TextSettings textSettings in settings.TextToDraw)
            {
                richString
                     .Alignment(textSettings.Alignment == "right"
                         ? TextAlignment.Right
                         : textSettings.Alignment == "left"
                             ? TextAlignment.Left
                             : textSettings.Alignment == "center"
                                 ? TextAlignment.Center
                                 : TextAlignment.Auto)
                     .FontFamily(textSettings.Font)
                     .FontItalic(textSettings.Style == "italic")
                     .FontSize(textSettings.Size)
                     .FontWeight(textSettings.Weight)
                     .LetterSpacing(textSettings.Spacing)
                     .StrikeThrough(textSettings.Style == "strikethrough" ? StrikeThroughStyle.Solid : StrikeThroughStyle.None)
                     .TextColor(SKColor.TryParse(textSettings.Color, out SKColor color) ? color : SKColor.Parse("#00000000"))
                     .Underline(textSettings.Style == "underline" ? UnderlineStyle.Solid : UnderlineStyle.None)
                     .Add(textSettings.Text);
            }
            SKImageInfo imageInfo = new(Convert.ToInt32(richString.MeasuredWidth), Convert.ToInt32(richString.MeasuredHeight));

            using SKSurface surface = SKSurface.Create(imageInfo);
            SKCanvas canvas = surface.Canvas;
            richString.Paint(canvas, new SKPoint(0, 0));

            using SKImage image = surface.Snapshot();
            using SKData data = image.Encode(SKEncodedImageFormat.Png, 100);
            using FileStream fileStream = File.OpenWrite("generated.png");
            data.SaveTo(fileStream);
        }

        public static List<List<string>> ParsePixels(SKBitmap bitmap, Settings settings)
        {
            List<List<string>> pixels = new();
            using (SKBitmap _ = SKBitmap.Decode(settings.TextToDraw != null ? "generated.png" : settings.FileName))
            {
                pixels = bitmap.Pixels.Select(x => settings.ColorsToIgnore?.Any(y => x.WithAlpha(255).ToString() == y) == true || x.Alpha == 0 ? null : $"0x{x.ToString()[1..].ToUpper()}").Chunk(bitmap.Width).Select(x => x.ToList()).ToList();
            }

            return pixels;
        }

        public static (double, double) GetOriginCoordinates(SKBitmap bitmap, Settings settings)
        {
            return ((-bitmap.Width / (2 / settings.PixelSize)) + settings.OriginDistance.Z, Math.Abs((bitmap.Height / (2 / settings.PixelSize)) - settings.OriginDistance.X));
        }
    }
}