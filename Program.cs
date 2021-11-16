using System.Text.Json;
using Img2Dsj.Models;
using Img2Dsj.Utils;
using SkiaSharp;

Settings settings = new();
try
{
    settings = JsonSerializer.Deserialize<Settings>(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "settings.json")), new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
}
catch (JsonException e)
{
    Console.WriteLine($"Invalid settings.json file: {e.Message}");
}

if (settings.TextToDraw != null)
{
    BitmapUtils.DrawText(settings);
}
using SKBitmap initialBitmap = SKBitmap.Decode(settings.TextToDraw != null ? "generated.png" : settings.FileName);
using SKBitmap resizedBitmap = initialBitmap.Resize(new SKImageInfo(Convert.ToInt32(initialBitmap.Width / settings.ScalingFactor), Convert.ToInt32(initialBitmap.Height / settings.ScalingFactor)), SKFilterQuality.High);
XmlUtils.GenerateMarkings(resizedBitmap, settings);

if(settings.TextToDraw != null) {
    File.Delete("generated.png");
}