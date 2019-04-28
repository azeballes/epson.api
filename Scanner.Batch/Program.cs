using log4net;
using Scanner.Driver;
using System;
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
            while (true)
            {
                try
                {
                    foreach (var document in scanner.Documents)
                    {
                        ProcessDocument(document, scanner);
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

        private static void ProcessDocument(Document document, IDigitalizer scanner)
        {
            while (true)
            {
                try
                {
                    scanner.Documents.Remove(document);
                    break;
                }
                catch(Exception ex)
                {
                    Log.Error($"No se puede guardar el docomento #{document.Id}: {ex.Message}");
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
