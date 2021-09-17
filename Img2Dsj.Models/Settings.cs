using System.Text.Json.Serialization;

namespace Img2Dsj.Models
{
    public class Settings
    {
        [JsonPropertyName("fileName")]
        public string FileName { get; set; }
        [JsonPropertyName("pixelSize")]
        public double PixelSize { get; set; }
        [JsonPropertyName("scalingFactor")]
        public int ScalingFactor { get; set; }
        [JsonPropertyName("originDistance")]
        public OriginDistance OriginDistance { get; set; }
        [JsonPropertyName("ignoreColor")]
        public string ColorToIgnore { get; set; }
        [JsonPropertyName("includeTags")]
        public string[] TagsToInclude { get; set; }
        [JsonPropertyName("winterMode")]
        public string WinterMode { get; set; }
    }

    public class OriginDistance
    {
        [JsonPropertyName("x")]
        public float X { get; set; }
        [JsonPropertyName("z")]
        public float Z { get; set; }
    }
}