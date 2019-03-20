using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using Scanner.Driver;

namespace Scanner.Api
{
    public class Scanner : IScanner
    {
        static IDigitalizer _digitalizer;

        public Scanner()
        {
            var context = System.ServiceModel.Web.WebOperationContext.Current;
            if (context == null) return;
            context.OutgoingResponse.Headers.Add("Access-Control-Allow-Origin", "*");
            context.OutgoingResponse.Headers.Add("Access-Control-Allow-Methods", "*");
            context.OutgoingResponse.Headers.Add("Access-Control-Allow-Headers", "*");
        }

        public Result Connect() => ConnectionAction(@"Conectar", "Conexión OK", () =>
        {
            if (_digitalizer != null) 
                return;
            _digitalizer = new EpsonDigitalizer();
            _digitalizer.Connect();
        });

        public Result Scan() => ConnectionAction(@"Digitalizar", "Digitalización OK", () =>
        {
            if (_digitalizer == null)
                throw new Exception("Digitalizadora no conectada");
            _digitalizer.Scan();
        });

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

        public Result Disconnect() => ConnectionAction(@"Desconectar", "Desconexión OK", () =>
        {
            if (_digitalizer == null)
                return;
            _digitalizer.Disconnect();
            _digitalizer = null;
        });

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
            if (_digitalizer == null)
                return new Document[] { };
            var ret = new List<Document>();
            foreach (var digitalizerDocument in _digitalizer.Documents)
            {
                ret.Add(new Document(digitalizerDocument));
            }
            return ret.ToArray();
        }

        
    }
}
