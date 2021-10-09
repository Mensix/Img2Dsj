using System.Text.Json.Serialization;

namespace Img2Dsj.Models
{
    public class Settings
    {
        public string FileName { get; set; }
        [JsonPropertyName("drawText")]
        public TextSettings TextToDraw { get; set; }
        public double PixelSize { get; set; } = 0.1;
        public int ScalingFactor { get; set; } = 5;
        public OriginDistance OriginDistance { get; set; }
        [JsonPropertyName("ignoreColors")]
        public string[] ColorsToIgnore { get; set; }
        [JsonPropertyName("includeTags")]
        public string[] TagsToInclude { get; set; }
    }

    public class TextSettings
    {
        public string Text { get; set; }
        public string Font { get; set; }
        public string Color { get; set; }
        public int Size { get; set; }
        public string Style { get; set; }
    }

    public class OriginDistance
    {
        public float X { get; set; } = 50;
        public float Z { get; set; } = 0;
    }
}