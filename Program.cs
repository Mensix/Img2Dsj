using System.Drawing;
using System.IO;
using System.Text.Json;
using Img2Dsj.Models;
using Img2Dsj.Utils;

Settings settings = JsonSerializer.Deserialize<Settings>(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "settings.json")), new()
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
});

if (settings.FileName != null)
{
    using Bitmap initialBitmap = (Bitmap)Image.FromFile(Path.Combine(Directory.GetCurrentDirectory(), settings.FileName));
    using Bitmap bitmap = BitmapUtils.ResizeImage(initialBitmap, initialBitmap.Width / settings.ScalingFactor, initialBitmap.Height / settings.ScalingFactor);
    XmlUtils.GenerateMarkings(bitmap, settings);
}
else if (settings.TextToDraw != null)
{
    using Bitmap initialBitmap = BitmapUtils.DrawTextImage(settings);
    using Bitmap bitmap = BitmapUtils.ResizeImage(initialBitmap, initialBitmap.Width / settings.ScalingFactor, initialBitmap.Height / settings.ScalingFactor);
    XmlUtils.GenerateMarkings(bitmap, settings);
}