using Scanner.Api;
using System;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Scanner.Host
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var uriString = System.Configuration.ConfigurationManager.AppSettings["ServiceUri"];
                var baseAddress = new Uri(uriString);
                var host = new WebServiceHost(typeof(Api.Scanner), baseAddress);
                host.AddServiceEndpoint(typeof(IScanner), new WebHttpBinding(), "");
                
                host.Open();
                Console.WriteLine($@"Scanner esperando peticiones en {baseAddress.AbsoluteUri}" );
                PressAnyKeyToQuit();
                host.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                PressAnyKeyToQuit();
            }
        }

        private static void PressAnyKeyToQuit()
        {
            Console.WriteLine(@"Presione 'Q' para salir...");
            while (true)
            {
                var key = Console.ReadKey();
                if (key.KeyChar.ToString().ToUpper().Equals("Q"))
                    break;
            }
        }
    }
}
