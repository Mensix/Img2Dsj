using System.Drawing;
using System.Text.Json.Serialization;

namespace Img2Dsj
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
        public int OriginDistance { get; set; }
        [JsonPropertyName("colorToIgnore")]
        public string ColorToIgnore { get; set; }
    }
}