using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Scanner.Driver
{
    internal static class TiffUtil
    {
        private static ImageCodecInfo _tifImageCodecInfo;
        private static EncoderParameter _tifEncoderParameterMultiFrame;
        private static readonly Encoder tifEncoderSaveFlag = Encoder.SaveFlag;
        private static EncoderParameters _tifEncoderParametersPage1;
        private static EncoderParameters _tifEncoderParametersPageX;
        private static EncoderParameters _tifEncoderParametersPageLast;
        private static EncoderParameter _tifEncoderParameterFrameDimensionPage;
        private static EncoderParameter _tifEncoderParameterFlush;
        private static EncoderParameter _tifEncoderParameterLastFrame;

        private static readonly Encoder TifEncoderCompression = Encoder.Compression;
        private static readonly Encoder TifEncoderColorDepth = Encoder.ColorDepth;

        private static EncoderParameter _tifEncoderParameter1Bpp;
        private static EncoderParameter tifEncoderParameterCompression;

        private static void AssignEncoder()
        {
            if (_tifImageCodecInfo == null)
            {
                foreach (ImageCodecInfo ici in ImageCodecInfo.GetImageEncoders())
                {
                    if (ici.MimeType == "image/tiff")
                    {
                        _tifImageCodecInfo = ici;
                        break;
                    }
                }

                _tifEncoderParameterFlush = new EncoderParameter(tifEncoderSaveFlag, (long)EncoderValue.Flush);
                _tifEncoderParameterMultiFrame = new EncoderParameter(tifEncoderSaveFlag, (long)EncoderValue.MultiFrame);
                _tifEncoderParameterFrameDimensionPage = new EncoderParameter(tifEncoderSaveFlag, (long)EncoderValue.FrameDimensionPage);
                _tifEncoderParameter1Bpp = new EncoderParameter(TifEncoderColorDepth, (long)8);
                tifEncoderParameterCompression = new EncoderParameter(TifEncoderCompression, (long)EncoderValue.CompressionCCITT4);
                _tifEncoderParameterLastFrame = new EncoderParameter(tifEncoderSaveFlag, (long)EncoderValue.LastFrame);
                /*
                //Regular
                
                tifEncoderParametersPage1 = new EncoderParameters(1);
                tifEncoderParametersPage1.Param[0] = tifEncoderParameterMultiFrame;
                tifEncoderParametersPageX = new EncoderParameters(1);

                tifEncoderParametersPageX.Param[0] = tifEncoderParameterFrameDimensionPage;
                tifEncoderParametersPageLast = new EncoderParameters(1);                
                tifEncoderParametersPageLast.Param[0] = tifEncoderParameterFlush;
                */

                // 1 BPP BW 
                _tifEncoderParametersPage1 = new EncoderParameters(2);
                _tifEncoderParametersPage1.Param[0] = _tifEncoderParameterMultiFrame;
                _tifEncoderParametersPage1.Param[1] = tifEncoderParameterCompression;
                _tifEncoderParametersPageX = new EncoderParameters(2);
                _tifEncoderParametersPageX.Param[0] = _tifEncoderParameterFrameDimensionPage;
                _tifEncoderParametersPageX.Param[1] = tifEncoderParameterCompression;
                _tifEncoderParametersPageLast = new EncoderParameters(2);
                _tifEncoderParametersPageLast.Param[0] = _tifEncoderParameterFlush;
                _tifEncoderParametersPageLast.Param[1] = _tifEncoderParameterLastFrame;
            }
        }

        public static Byte[] mergeTiffPages(Byte[][] aux)
        {
            if (aux == null)
                return null;

            if (aux.Length <= 1)
                return aux[0];
            AssignEncoder();
            MemoryStream ms = new MemoryStream();
            Bitmap front = new Bitmap(new MemoryStream(aux[0]));
            Bitmap rear = null;
            front.Save(ms, _tifImageCodecInfo, _tifEncoderParametersPage1);
            for (int x = 1; x < aux.Length; x++)
            {
                rear = new Bitmap(new MemoryStream(aux[x]));
                front.SaveAdd(rear, _tifEncoderParametersPageX);
            }
            front.SaveAdd(_tifEncoderParametersPageLast);
            front.Dispose();
            if (rear != null) rear.Dispose();
            return ms.ToArray();
        }
    }
}