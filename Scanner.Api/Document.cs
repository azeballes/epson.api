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
            const string dataImageJpegBase64 = "data:image/jpeg;base64,";
            Base64FrontImage = dataImageJpegBase64 + Convert.ToBase64String(document.RawImageFront);
            Base64BackImage = dataImageJpegBase64 + Convert.ToBase64String(document.RawImageBack);
        }
        public int Id { get; set; }
        public string Cmc7 { get; set; }
        public string Base64FrontImage { get; set; }
        public string Base64BackImage { get; set; }

    }
}