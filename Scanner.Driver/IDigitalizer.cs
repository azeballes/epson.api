using System.Collections.Generic;

namespace Scanner.Driver
{
    public interface IDigitalizer
    {
        void Connect();
        void Disconnect();
        //void Scan();
        IList<Document> Documents { get; }
    }
}
