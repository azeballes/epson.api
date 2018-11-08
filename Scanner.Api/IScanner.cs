using System.ServiceModel;
using System.ServiceModel.Web;

namespace Scanner.Api
{
    [ServiceContract]
    public interface IScanner
    {

        [OperationContract]
        [WebGet(UriTemplate = "estado", ResponseFormat = WebMessageFormat.Json)]
        Result ConnectionStatus();

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "conectar", ResponseFormat = WebMessageFormat.Json)]
        Result Connect();

        [WebInvoke(Method = "POST", UriTemplate = "desconectar", ResponseFormat = WebMessageFormat.Json)]
        Result Disconnect();

        [OperationContract]
        [WebGet(UriTemplate = "documentos", ResponseFormat = WebMessageFormat.Json)]
        Document [] Documents();

    }
}
