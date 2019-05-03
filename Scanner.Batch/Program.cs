using log4net;
using Scanner.Data;
using Scanner.Driver;
using System;
using System.Configuration;
using System.Threading;

namespace Scanner.Batch
{
    class Program
    {
        static readonly int Delay = 5000;
        static readonly ILog Log = Logging.Log(typeof(Program));
        static void Main(string[] args)
        {
            Log.Info("Iniciando la digitalización presione <CTRL + C> para finalizar...");
            Thread.Sleep(Delay);

            while (true)
            {
                var scanner = Connect();
                ReadDocuments(scanner);
                Disconnect(scanner);
            }
        }

        private static void Disconnect(IDigitalizer scanner)
        {
            try
            {
                Log.Info("Desconectando la digitalizadora...");
                scanner.Disconnect();
            }
            catch (Exception ex)
            {
                Log.Warn(ex.Message);
            }
        }

        private static void ReadDocuments(IDigitalizer scanner)
        {
            Log.Info("Esperando documentos...");
            while (true)
            {
                try
                {
                    while( scanner.Documents.Count != 0)
                    {                        
                        var document = scanner.Documents[0];
                        //Log.Debug($"Procesando documento #{document.Id}");
                        ProcessDocument(document);
                        Log.Debug($"Limpieza documento #{document.Id}");
                        scanner.Documents.Remove(document);
                    }                    
                }
                catch(Exception ex)
                {
                    Log.Error(ex.Message);
                    break;
                }
                Thread.Sleep(Delay);
            }
        }

        private static void ProcessDocument(Driver.Document document)
        {            
            while (true)
            {
                try
                {
                    Log.Debug($"Procesando documento #{document.Id}");
                    var cs = ConfigurationManager.ConnectionStrings["MyContext"];
                    if ( cs == null)
                        throw new Exception("No se encuentra la cadena de conexión 'MyContext' a la base de datos");
                    
                    var context = new ScannerContext(cs.ToString());
                    context.Save( new Data.Document
                        {
                            Date = DateTime.Now
                            , BackImage = document.RawImageBack
                            , Cmc7 = document.Cmc7
                            , FrontImage = document.RawImageFront
                            , State = 0
                    } );                    
                    break;
                }
                catch(Exception ex)
                {
                    Log.Error($"No se puede guardar el documento #{document.Id}: {ex.Message}");
                }
                Thread.Sleep(Delay);
            }
        }

        private static IDigitalizer Connect()
        {
            while (true)
            {
                try
                {
                    Log.Info("Conectando la digitalizadora...");
                    IDigitalizer scanner = new EpsonDigitalizer();
                    //IDigitalizer scanner = new MockDigitalizer();
                    scanner.Connect();
                    Log.Info("Conección OK");
                    return scanner;
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message);
                }
                Thread.Sleep(Delay);
            }
        }
    }
}
