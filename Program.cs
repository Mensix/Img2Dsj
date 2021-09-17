﻿using System.Drawing;
using System.IO;
using System.Text.Json;
using Img2Dsj.Utils;
using Img2Dsj.Models;

Settings settings = JsonSerializer.Deserialize<Settings>(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "settings.json")));
using (Bitmap initialBitmap = (Bitmap)Image.FromFile(Path.Combine(Directory.GetCurrentDirectory(), settings.FileName)))
using (Bitmap bitmap = BitmapUtils.ResizeImage(initialBitmap, initialBitmap.Width / settings.ScalingFactor, initialBitmap.Height / settings.ScalingFactor))
{
    XmlUtils.GenerateMarkings(bitmap, settings);
}
