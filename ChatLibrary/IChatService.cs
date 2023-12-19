using ChatLibrary.DataContracts.ClientModels;
using ChatLibrary.DataContracts.Common;
using ChatLibrary.DataContracts.ServerModels;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace ChatLibrary
{
    [ServiceContract(CallbackContract = typeof(IChatServiceCallback))]
    public interface IChatService
    {

        // Two main methods to establish connection between client and server

        [OperationContract]
        ConnectResult Connect(string username);

        [OperationContract]
        ChatModel GetChatById(Guid chatId);

        [OperationContract(IsOneWay = true)]
        void Disconnect(ConnectContract contract);

        // Misc

        [OperationContract(IsOneWay = true)]
        void SendMessage(ConnectContract contract, ClientMessageModel message);

        [OperationContract]
        List<ClientModel> GetAllClients(ConnectContract contract);

        [OperationContract]
        ClientModel GetClientById(int id);

        // used to get conversation between sender and clientId
        [OperationContract]
        ChatModel GetConversation(ConnectContract contract, int clientId);

        // Debug, information purpose
        [OperationContract(IsOneWay = true)]
        void SendToServer(ConnectContract contract, string message);
    }
}
