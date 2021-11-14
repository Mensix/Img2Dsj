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

BitmapUtils.DrawText(settings);
if (settings.TextToDraw != null)
{
    BitmapUtils.DrawText(settings);
}
using SKBitmap _ = SKBitmap.Decode(settings.TextToDraw != null ? "generated.png" : settings.FileName);
XmlUtils.GenerateMarkings(_, settings);