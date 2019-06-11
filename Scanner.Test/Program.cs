using Scanner.Data;
using System;
using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Scanner.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            var context = new ScannerContext(ConfigurationManager.ConnectionStrings["MyContext"].ToString());

            context.Save( new Document {
                Date = DateTime.Now
                , BackImage = new byte [] { 0x01, 0x02}
                , Cmc7 = "cmc7"
                , FrontImage = new byte[] { 0x01, 0x02 }
                , State = 10
            } );
            */

            var imagen = new Bitmap(@"C:\Users\az22207\AppData\Local\Temp\636957227357067316_frente.jpg");
            var newImagen = Resize(imagen, imagen.Width / 4, imagen.Height / 4, true);
            newImagen.Save(@"C:\Users\az22207\AppData\Local\Temp\636957227357067316_frente_2.jpg");
        }

        public static Image Resize(Image image, int newWidth, int maxHeight, bool onlyResizeIfWider)
        {
            if (onlyResizeIfWider && image.Width <= newWidth) newWidth = image.Width;

            var newHeight = image.Height * newWidth / image.Width;
            if (newHeight > maxHeight)
            {
                // Resize with height instead  
                newWidth = image.Width * maxHeight / image.Height;
                newHeight = maxHeight;
            }

            var res = new Bitmap(newWidth, newHeight);

            using (var graphic = Graphics.FromImage(res))
            {
                graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphic.SmoothingMode = SmoothingMode.HighQuality;
                graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphic.CompositingQuality = CompositingQuality.HighQuality;
                graphic.DrawImage(image, 0, 0, newWidth, newHeight);
            }

            return res;
        }
    }
}
