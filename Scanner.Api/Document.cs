using System;

namespace Scanner.Api
{
    public class Document
    {
        public Document(){}

        public Document(Driver.Document document)
        {
            Id = document.Id;
            Cmc7 = document.Cmc7;
            //Base64Image = "data:image/jpeg;base64, " + Convert.ToBase64String(document.RawImage); ;
            Base64Image = "data:image/tiff;base64, " + Convert.ToBase64String(document.RawImage); ; 
        }
        public int Id { get; set; }
        public string Cmc7 { get; set; }
        public string Base64Image { get; set; }

    }
}