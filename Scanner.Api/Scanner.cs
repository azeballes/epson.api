using System;
using System.Drawing.Imaging;
using System.IO;
using Scanner.Driver;

namespace Scanner.Api
{
    public class Scanner : IScanner
    {
        private IDigitalizer _digitalizer = new EpsonDigitalizer();

        public Scanner()
        {
            var context = System.ServiceModel.Web.WebOperationContext.Current;
            if (context == null) return;
            context.OutgoingResponse.Headers.Add("Access-Control-Allow-Origin", "*");
            context.OutgoingResponse.Headers.Add("Access-Control-Allow-Methods", "*");
            context.OutgoingResponse.Headers.Add("Access-Control-Allow-Headers", "*");
        }

        public Result Connect() => ConnectionAction(@"Conectar", "Conexión OK", () => _digitalizer.Connect());
        
        private static Result Result(string description, int code)
        {
            var result = new Result
            {
                Code = code,
                Description = description
            };
            return result;
        }

        public Result ConnectionStatus()
        {
            Console.WriteLine(@"Status");
            var result = new Result
            {
                Code = 0,
                Description = "Estado de la conexión"
            };
            return result;
        }

        public Result Disconnect() => ConnectionAction(@"Desconectar", "Desconexión OK", () => _digitalizer.Disconnect());

        private static Result ConnectionAction(string message, string messageOk, Action action)
        {
            Console.WriteLine(message);
            try
            {
                action();
                Console.WriteLine(messageOk);
                return Result(messageOk, 0);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                return Result(exception.Message, -1);
            }
        }

        public Document [] Documents()
        {
            Console.WriteLine(@"Documents");
            return new []
            {
                new Document()
                {
                    Id = 1,
                    Cmc7 = Resource1.Cmc7_1,
                    Image = Base64Image()
                }
            };
        }

        private static string Base64Image()
        {
            var memoryStream = new MemoryStream();
            Resource1.ChequeJPG.Save(memoryStream, ImageFormat.Jpeg);
            var buffer = new byte[memoryStream.Length];
            memoryStream.Position = 0;
            memoryStream.Read(buffer, 0, buffer.Length);
            memoryStream.Close();
            return "data:image/jpeg;base64, " + Convert.ToBase64String(buffer);
        }
    }

    public class Document
    {
        public int Id { get; set; }
        public string Cmc7 { get; set; }
        public string Image { get; set; }
    }
}
