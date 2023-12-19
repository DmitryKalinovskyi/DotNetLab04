using ChatLibrary.DataContracts.Common;
using ChatLibrary.DataContracts.ServerModels;
using System.ServiceModel;

namespace ChatLibrary
{
    public interface IChatServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void ReceiveMessage(ConnectContract contract, ServerMessageModel message);

        [OperationContract(IsOneWay = true)]
        void OnClientConnect(ConnectContract contract, ClientModel client);

        [OperationContract(IsOneWay = true)]
        void OnClientDisconnect(ConnectContract contract, ClientModel client);

        // Debug, information purpose only
        [OperationContract(IsOneWay = true)]
        void ReceiveServerMessage(ConnectContract contract, string message);
    }
}
