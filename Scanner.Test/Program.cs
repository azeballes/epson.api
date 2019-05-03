using Scanner.Data;
using System;
using System.Configuration;

namespace Scanner.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var context = new ScannerContext(ConfigurationManager.ConnectionStrings["MyContext"].ToString());

            context.Save( new Document {
                Date = DateTime.Now
                , BackImage = new byte [] { 0x01, 0x02}
                , Cmc7 = "cmc7"
                , FrontImage = new byte[] { 0x01, 0x02 }
                , State = 10
            } );

        }
    }
}
