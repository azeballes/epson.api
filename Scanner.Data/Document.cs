using System;

namespace Scanner.Data
{
    public class Document
    {
        public long Id { get; set; }

        public DateTime Date { get; set; }

        public string Cmc7 { get; set; }

        public byte[] FrontImage { get; set; }
        
        public byte[] BackImage { get; set; }
 
        public long State { get; set; }
 
    }
}