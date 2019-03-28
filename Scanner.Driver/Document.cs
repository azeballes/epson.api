namespace Scanner.Driver
{
    public class Document
    {
        public int Id { get; set; }
        public string Cmc7 { get; set; }
        public byte[] RawImageFront { get; set; }
        public byte[] RawImageBack { get; set; }
    }
}