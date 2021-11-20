using System.Text.Json.Serialization;

namespace Img2Dsj.Models
{
    public class Settings
    {
        public string FileName { get; set; }
        [JsonPropertyName("drawText")]
        public TextSettings[] TextToDraw { get; set; }
        public double PixelSize { get; set; } = 0.1;
        public double ScalingFactor { get; set; } = 1;
        public OriginDistance OriginDistance { get; set; }
        [JsonPropertyName("ignoreColors")]
        public string[] ColorsToIgnore { get; set; }
        [JsonPropertyName("includeTags")]
        public string[] TagsToInclude { get; set; }
        [JsonPropertyName("useColor")]
        public string ColorToUse { get; set; }
    }

    public class TextSettings
    {
        public int Size { get; set; }
        public int Spacing { get; set; }
        public int Weight { get; set; } = 400;
        public string Alignment { get; set; } = "left";
        public string Color { get; set; } = "#000000";
        public string Font { get; set; }
        public string Style { get; set; } = "regular";
        public string Text { get; set; }
    }

    public class OriginDistance
    {
        public float X { get; set; } = 50;
        public float Z { get; set; } = 0;
    }
}