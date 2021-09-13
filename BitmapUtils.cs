using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Img2Dsj
{
    public static class BitmapUtils
    {
        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            Rectangle rectangle = new(0, 0, width, height);
            Bitmap bitmap = new(width, height);
            bitmap.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using ImageAttributes imageAttributes = new();
                imageAttributes.SetWrapMode(WrapMode.TileFlipXY);
                graphics.DrawImage(image, rectangle, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
            }

            return bitmap;
        }
    }
}