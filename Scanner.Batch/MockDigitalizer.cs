using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using Scanner.Batch.Properties;
using Scanner.Driver;

namespace Scanner.Batch
{
    public class MockDigitalizer : IDigitalizer
    {

        IList<Document> _documents;

        public MockDigitalizer()
        {
            _documents = new List<Document>();
            var t = new Thread(ScanDocument);
            t.Start();
        }

        private void ScanDocument() {
            var id = 1;
            while (true) {                
                Documents.Add(
                    new Document
                    {
                        Id = id++
                        ,
                        Cmc7 = "<1231231234123456781234568901>"
                        ,
                        RawImageBack = GetByteArrayFromImage(Resources.dorso)
                        ,
                        RawImageFront = GetByteArrayFromImage(Resources.frente)
                    });
                Thread.Sleep(6000);
            }
        }

        private byte[] GetByteArrayFromImage(Bitmap bmp)
        {
            MemoryStream ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Jpeg);
            return ms.ToArray();
        }

        public IList<Document> Documents { get {
                lock ("Documents") {
                    return _documents;
                }
            }
        }

        public void Connect()
        {
            
        }

        public void Disconnect()
        {
            
        }
    }
}
