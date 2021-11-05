using System.Collections.Generic;
using System.Xml.Serialization;

namespace Img2Dsj.Models
{
    public class Banner
    {
        [XmlAttribute("d1")]
        public double D1 { get; set; }
        [XmlAttribute("d2")]
        public double D2 { get; set; }
        [XmlAttribute("z1")]
        public double Z1 { get; set; }
        [XmlAttribute("z2")]
        public double Z2 { get; set; }
        [XmlAttribute("c")]
        public string C { get; set; }
        [XmlAttribute("w")]
        public double W { get; set; }
        [XmlAttribute("side")]
        public string Side { get; set; } = "custom";
    }

    public class Line
    {
        [XmlAttribute("d")]
        public double D { get; set; }
        [XmlAttribute("z1")]
        public double Z1 { get; set; }
        [XmlAttribute("z2")]
        public double Z2 { get; set; }
        [XmlAttribute("c")]
        public string C { get; set; }
        [XmlAttribute("w")]
        public double W { get; set; }
    }

    public class Spray : Line { }

    public class Twigs
    {
        [XmlAttribute("d")]
        public double D { get; set; }
        [XmlAttribute("z1")]
        public double Z1 { get; set; }
        [XmlAttribute("z2")]
        public double Z2 { get; set; }
        [XmlAttribute("forcedetailed")]
        public bool ForceDetailed = true;
    }

    [XmlRoot("summer")]
    public class Summer
    {
        [XmlElement("banner")]
        public List<Banner> Banners { get; set; }
        [XmlElement("line")]
        public List<Line> Lines { get; set; }
    }

    [XmlRoot("winter")]
    public class Winter
    {
        [XmlElement("spray")]
        public List<Spray> Sprays { get; set; }
        [XmlElement("twigs")]
        public List<Twigs> Twigs { get; set; }
    }

    [XmlRoot("custom-markings")]
    public class Marking
    {
        [XmlElement("summer")]
        public Summer Summer { get; set; }
        [XmlElement("winter")]
        public Winter Winter { get; set; }
    }
}