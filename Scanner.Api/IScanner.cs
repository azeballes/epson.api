using System.ServiceModel;
using System.ServiceModel.Web;
using Scanner.Driver;

namespace Scanner.Api
{
    [ServiceContract]
    public interface IScanner
    {

        [OperationContract]
        [WebGet(UriTemplate = "estado", ResponseFormat = WebMessageFormat.Json)]
        Result ConnectionStatus();

        [OperationContract]
        [WebGet(UriTemplate = "conectar", ResponseFormat = WebMessageFormat.Json)]
        Result Connect();

        [OperationContract]
        //[WebInvoke(Method = "POST", UriTemplate = "conectar", ResponseFormat = WebMessageFormat.Json)]
        [WebGet(UriTemplate = "digitalizar", ResponseFormat = WebMessageFormat.Json)]
        Result Scan();

        [OperationContract]
        [WebGet(UriTemplate = "desconectar", ResponseFormat = WebMessageFormat.Json)]
        Result Disconnect();

        [OperationContract]
        [WebGet(UriTemplate = "documentos", ResponseFormat = WebMessageFormat.Json)]
        Document [] Documents();

    }
}
