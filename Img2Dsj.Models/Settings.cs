using System.Text.Json.Serialization;

namespace Img2Dsj.Models
{
    public class Settings
    {
        public string FileName { get; set; }
        public double PixelSize { get; set; } = 0.1;
        public int ScalingFactor { get; set; } = 5;
        public OriginDistance OriginDistance { get; set; }
        public string[] ColorsToIgnore { get; set; }
        public string[] TagsToInclude { get; set; }
    }

    public class OriginDistance
    {
        public float X { get; set; } = 50;
        public float Z { get; set; } = 0;
    }
}