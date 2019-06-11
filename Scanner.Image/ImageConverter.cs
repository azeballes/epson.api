using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace Scanner.Image
{
    public class ImageConverter
    {
        private readonly System.Drawing.Image _image;

        public ImageConverter(byte [] rawImage)
        {
            var ms = new MemoryStream(rawImage);
            _image = new Bitmap(ms);
            ms.Dispose();
        }

        public byte [] Resize(double factor = 0.33D)
        {

            var newHeight = (int) (_image.Height * factor);
            var newWidth = (int) (_image.Width * factor);

            var res = new Bitmap(newWidth, newHeight);

            using (var graphic = Graphics.FromImage(res))
            {
                graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphic.SmoothingMode = SmoothingMode.HighQuality;
                graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphic.CompositingQuality = CompositingQuality.HighQuality;
                graphic.DrawImage(_image, 0, 0, newWidth, newHeight);
            }

            return RawImageBuffer(res);
        }

        private static byte[] RawImageBuffer(System.Drawing.Image res)
        {
            var memoryStream = new MemoryStream();
            res.Save(memoryStream, ImageFormat.Jpeg);
            return memoryStream.GetBuffer();
        }
    }
}
